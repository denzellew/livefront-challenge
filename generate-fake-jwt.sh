#!/bin/bash

# Usage: ./generate_fake_jwt.sh <user_id>
# Example: ./generate_fake_jwt.sh abcxyzcc-1234-5678-90ab-cdefghijklmnop

USER_ID=$1
if [ -z "$USER_ID" ]; then
  USER_ID=$(uuidgen)
  echo "User ID Generated: $USER_ID"
fi

HEADER='{"alg":"HS256","typ":"JWT"}'
CURRENT_TIME=$(date +%s)
PAYLOAD=$(jq -c -n --arg sub "$USER_ID" --argjson iat "$CURRENT_TIME" '{sub: $sub, iat: $iat}')

echo "Payload: $PAYLOAD"
b64url_encode() {
  printf '%s' "$1" | base64 | tr -d '\n=' | tr '+/' '-_'
}

HEADER_B64=$(b64url_encode "$HEADER")
PAYLOAD_B64=$(b64url_encode "$PAYLOAD")
SIGNATURE=$(b64url_encode "fake-signature")

FAKE_JWT="${HEADER_B64}.${PAYLOAD_B64}.${SIGNATURE}"

echo "User ID: $USER_ID"
echo "JWT: $FAKE_JWT"