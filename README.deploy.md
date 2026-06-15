## How to deploy
1. Application is hosted in the cloud in AWS Light Sail container, to deploy run script.  
```aiignore
cd <root-folder>
# Example: ./scripts/deploy-notifications.sh            # Auto-increment version, build, push, and deploy
# Example: ./scripts/deploy-notifications.sh v1.2.3     # Build, push, and deploy specific version
```

## Test Deployment
```aiignore
curl -v https://derek-notifications-container-service.n0283r19w2876.us-east-1.cs.amazonlightsail.com/info/health
```
