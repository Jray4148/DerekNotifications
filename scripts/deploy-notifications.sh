#!/bin/bash

# deploy-notifications.sh
# Usage: ./scripts/deploy-notifications.sh [version_tag]
# Example: ./scripts/deploy-notifications.sh            # Auto-increment version, build, push, and deploy
# Example: ./scripts/deploy-notifications.sh v1.2.3     # Build, push, and deploy specific version

TAG=$1

# Configuration variables
ECR_REPO_NAME="rsi/derek-notifications"
AWS_REGION="us-east-1"

# Function to get and increment version tag
get_next_version() {
    echo "🔍 Fetching latest tag from GitHub..." >&2
    
    # Fetch all tags from remote
    git fetch --tags >&2 2>/dev/null
    
    # Get the latest tag (assumes semantic versioning like v1.2.3)
    LATEST_TAG=$(git tag -l "v*" | sort -V | tail -n 1)
    
    if [ -z "$LATEST_TAG" ]; then
        echo "⚠️  No existing tags found. Starting with v0.0.1" >&2
        echo "v0.0.1"
        return
    fi
    
    echo "📌 Latest tag: $LATEST_TAG" >&2
    
    # Extract version numbers (remove 'v' prefix)
    VERSION=${LATEST_TAG#v}
    
    # Split version into components
    IFS='.' read -r MAJOR MINOR PATCH <<< "$VERSION"
    
    # Increment patch version
    PATCH=$((PATCH + 1))
    
    # Create new version tag
    NEW_TAG="v${MAJOR}.${MINOR}.${PATCH}"
    echo "✨ Next version will be: $NEW_TAG" >&2
    echo "$NEW_TAG"
}

# If TAG is not provided, auto-generate next version
if [ -z "$TAG" ]; then
    echo "🔍 No tag provided. Checking git for latest version..."
    TAG=$(get_next_version)
    echo "🏷️  Using auto-generated tag: $TAG"
fi

# Get AWS account ID dynamically
AWS_ACCOUNT_ID=$(aws sts get-caller-identity --query Account --output text)
IMAGE_URI=$AWS_ACCOUNT_ID.dkr.ecr.$AWS_REGION.amazonaws.com/$ECR_REPO_NAME:$TAG

## Docker Build and Push Helper Function
# ------------------------------------
docker_build_and_push() {
    docker buildx build --platform linux/amd64 -t "$IMAGE_URI" --push . --provenance=false
}

## ECR Login Function
# -----------------
ecr_login() {
    echo "Attempting AWS ECR login..."
    if ! aws ecr get-login-password --region $AWS_REGION | docker login --username AWS --password-stdin "$AWS_ACCOUNT_ID.dkr.ecr.$AWS_REGION.amazonaws.com"; then
        echo "❌ Failed to log in to AWS ECR. Check AWS credentials/permissions."
        exit 1
    fi
}

## Docker Build and Push Function
# -----------------------------
build_and_push() {
    echo "📦 Now building and pushing **Docker image** with tag **$TAG**..."
    
    # Try build/push first
    if ! docker_build_and_push; then
        echo "Initial build/push failed, attempting AWS ECR login..."
        ecr_login
        
        echo "Retrying build/push..."
        if ! docker_build_and_push; then
            echo "❌ Failed to build and push image to ECR on retry."
            exit 1
        fi
    fi

    # Also tag and push as "latest" for team deployments
    if [ "$TAG" != "latest" ]; then
        echo "📦 Also tagging and pushing as **latest** for team deployments..."
        LATEST_URI="$AWS_ACCOUNT_ID.dkr.ecr.$AWS_REGION.amazonaws.com/$ECR_REPO_NAME:latest"

        # Tag the image as latest
        docker tag "$IMAGE_URI" "$LATEST_URI"

        # Push the latest tag
        if ! docker push "$LATEST_URI"; then
            echo "❌ Failed to push latest tag."
            exit 1
        fi

        echo "✅ Also pushed as **latest** tag!"
    fi

    echo "✅ Docker image **$IMAGE_URI** successfully built and pushed to ECR!"
    
    # Tag and push to GitHub
    echo "🏷️  Attempting to create and push Git tag $TAG to GitHub..."
    git tag "$TAG"
    if [ $? -ne 0 ]; then
        echo "⚠️  Warning: Failed to create Git tag. Tag may already exist."
        echo "Attempting to push existing tag..."
    fi
    
    git push origin "$TAG"
    if [ $? -ne 0 ]; then
        echo "❌ Failed to push Git tag to GitHub. Exiting."
        exit 1
    fi
    
    echo "✅ Git tag **$TAG** successfully pushed to GitHub!"    
}


## Terraform Config Update and Apply Function
# ------------------------------------------
config_and_apply() {
    local TERRAFORM_DIR="terraform"
    local DEPLOY_TAG="${1:-latest}"  # Use provided tag or default to "latest"
    
    echo "🚀 Starting **Terraform apply** with image tag: **$DEPLOY_TAG**..."
    
    # Change to terraform directory
    cd "$TERRAFORM_DIR" || { echo "❌ Failed to change to terraform directory"; exit 1; }
    
    # Determine terraform apply strategy based on tag
    if [ "$DEPLOY_TAG" = "latest" ]; then
        echo "⚠️  Using 'latest' tag - forcing deployment recreation to pull new image..."
        # Force recreation of deployment when using "latest" tag since tag name doesn't change
        APPLY_CMD="terraform apply -replace=aws_lightsail_container_service_deployment_version.container_service_deployment -auto-approve"
    else
        echo "📌 Using versioned tag - Terraform will detect the change..."
        # Use versioned tag via variable override
        APPLY_CMD="terraform apply -var=\"image_tag=$DEPLOY_TAG\" -auto-approve"
    fi
    
    # Apply the configuration
    if eval "$APPLY_CMD"; then
        echo ""
        echo "🎉 Deployment completed successfully!"
        echo "Your application is now live with image tag: **$DEPLOY_TAG**"
        echo ""
        echo "📊 Deployment outputs:"
        terraform output
    else
        echo "❌ Terraform deployment failed"
        exit 1
    fi
}


## Main Execution
# --------------

echo "🔄 Building, pushing, and deploying version $TAG..."
build_and_push
echo ""
echo "🔄 Now deploying to Lightsail..."
config_and_apply "$TAG"

exit 0