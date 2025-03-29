#!/bin/sh

# Generate a random UUID
USER_ID=$(od -x /dev/urandom | head -1 | awk '{OFS="-"; print $2$3,$4,$5,$6,$7$8$9}')

# Create header and payload
HEADER='{"alg":"HS256","typ":"JWT"}'
PAYLOAD="{\"sub\":\"$USER_ID\",\"iat\":$(date +%s),\"exp\":$(( $(date +%s) + 3600 ))}"

# Base64 encode header and payload
HEADER_B64=$(echo -n "$HEADER" | base64 | tr -d '=' | tr '/+' '_-')
PAYLOAD_B64=$(echo -n "$PAYLOAD" | base64 | tr -d '=' | tr '/+' '_-')

# Create signature (dummy signature for testing)
SIGNATURE="dummy_signature"

# Combine to create JWT
echo "$HEADER_B64.$PAYLOAD_B64.$SIGNATURE" 