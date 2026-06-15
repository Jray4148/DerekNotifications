variable "image_tag" {
  description = "Docker image tag to deploy"
  type        = string
  default     = "latest"
}

variable "app_name" {
  description = "Name of the application (used for resource naming)"
  type        = string
  default     = "derek-notifications"
}

variable "aws_region" {
  description = "AWS region for resources"
  type        = string
  default     = "us-east-1"
}

variable "ecr_repository_name" {
  description = "Name of the ECR repository"
  type        = string
  default     = "rsi/derek-notifications"
}


variable "rsi_api_token" {
  description = "Rsi legacy endpoints API token"
  type        = string
  sensitive   = true
}

variable "yt_api_token" {
  description = "YouTrack API token"
  type        = string
  sensitive   = true
}

variable "aws_access_key_id" {
  description = "AWS Access Key ID for the application"
  type        = string
  sensitive   = true
}

variable "aws_secret_access_key" {
  description = "AWS Secret Access Key for the application"
  type        = string
  sensitive   = true
}

variable "zoho_refresh_token" {
  description = "Zoho Refresh Token"
  type        = string
  sensitive   = true
}

variable "zoho_client_id" {
  description = "Zoho Client ID"
  type        = string
  sensitive   = true
}

variable "zoho_client_secret" {
  description = "Zoho Client Secret"
  type        = string
  sensitive   = true
}