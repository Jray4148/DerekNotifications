provider "aws" {
  region = "us-east-1"
}

# Computed names based on app_name
locals {
  container_name = "${var.app_name}-container"
}

## Repo already exists, do dont create.  If does not exist uncomment this block
# # ECR Repository for Docker images
# resource "aws_ecr_repository" "derek_notifications" {
#   name                 = var.ecr_repository_name
#   image_tag_mutability = "MUTABLE"
#   force_delete         = true

#   image_scanning_configuration {
#     scan_on_push = true
#   }

#   lifecycle {
#     prevent_destroy = false
#   }

#   tags = {
#     Name        = var.app_name
#     # Environment = var.environment
#   }
# }

# Reference existing ECR Repository instead of creating a new one
data "aws_ecr_repository" "app_repository" {
  name = var.ecr_repository_name
}

resource "aws_ecr_repository_policy" "lightsail_pull_policy" {
  repository = data.aws_ecr_repository.app_repository.name

  policy = jsonencode({
    Version = "2008-10-17",
    Statement = [
      {
        Sid       = "AllowLightsailPull",
        Effect    = "Allow",
        Principal = {
          AWS = aws_lightsail_container_service.container_service.private_registry_access[0].ecr_image_puller_role[0].principal_arn
        },
        Action = [
          "ecr:BatchCheckLayerAvailability",
          "ecr:GetDownloadUrlForLayer",
          "ecr:BatchGetImage",
          "ecr:GetAuthorizationToken" 
        ],
      }
    ]
  })
}

resource "aws_lightsail_container_service" "container_service" {
  name  = "${var.app_name}-container-service"
  power = "nano"  # Options: nano, micro, small, medium, large, xlarge
  scale = 1

  # Enable ECR access
  private_registry_access {
    ecr_image_puller_role {
      is_active = true
    }
  }

  # Attach SSL certificate to custom domain
  public_domain_names {
    certificate {
      certificate_name = aws_lightsail_certificate.derek_notifications_cert.name
      domain_names     = ["derek-notifications.rsiorg.com"]
    }
  }
}

# Create SSL/TLS certificate for your domain
resource "aws_lightsail_certificate" "derek_notifications_cert" {
  name        = "derek-notifications-cert"
  domain_name = "derek-notifications.rsiorg.com"
  
  # Optional: Add additional domains (like www subdomain)
  # subject_alternative_names = [
  #   "www.derek-notifications.rsiorg.com"
  # ]

  tags = {
    Environment = "production"
    Service     = "derek-notifications"
  }
}

# New CNAME record pointing to container service
resource "aws_route53_record" "derek_notifications_main" {
  zone_id = "Z00860031V3OO4FZ62WW7" # Route 53 zone ID
  name    = "derek-notifications.rsiorg.com"
  type    = "CNAME"
  ttl     = 300
  records = ["derek-notifications-container-service.n0283r19w2876.us-east-1.cs.amazonlightsail.com"]
}

# Certificate validation record - use these EXACT values
resource "aws_route53_record" "derek_notifications_cert_validation" {
  zone_id = "Z00860031V3OO4FZ62WW7"  # Route 53 zone ID
  name    = "_62b9353d615332696cd6a51ff1bba2c3.derek-notifications.rsiorg.com"
  type    = "CNAME"
  ttl     = 300
  records = ["_0aabe76d16d3ab4e35e7d47ccec80059.jkddzztszm.acm-validations.aws."]
}

# Enable custom domains on the container service
resource "aws_lightsail_domain" "derek_notifications_domain" {
  domain_name = "derek-notifications.rsiorg.com"
}

resource "aws_lightsail_container_service_deployment_version" "container_service_deployment" {
  depends_on = [aws_lightsail_certificate.derek_notifications_cert]
  container {
    container_name = local.container_name
    image          = "${data.aws_ecr_repository.app_repository.repository_url}:${var.image_tag}"

    ports = {
      "8080" = "HTTP"
    }

    environment = {
      AWS_REGION            = var.aws_region
      AWS_ACCESS_KEY_ID     = var.aws_access_key_id
      AWS_SECRET_ACCESS_KEY = var.aws_secret_access_key
      Yt__ApiToken          = var.yt_api_token
      Zoho__RefreshToken    = var.zoho_refresh_token
      Zoho__ClientId        = var.zoho_client_id
      Zoho__ClientSecret    = var.zoho_client_secret
      Rsi__ApiToken         = var.rsi_api_token
    }  
  }

  public_endpoint {
    container_name = local.container_name
    container_port = 8080
    
    health_check {
      healthy_threshold   = 2
      unhealthy_threshold = 3
      timeout_seconds     = 10
      interval_seconds    = 15
      path               = "/"
      success_codes      = "200-499"
    }
  }

  service_name = aws_lightsail_container_service.container_service.name
}

output "container_service_url" {
  description = "The URL of the deployed container service"
  value       = aws_lightsail_container_service.container_service.url
}

output "image_deployed" {
  description = "The Docker image currently deployed"
  value       = "${data.aws_ecr_repository.app_repository.repository_url}:${var.image_tag}"
}