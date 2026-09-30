#!/usr/bin/env bash
set -e

CONFIGURATION="${1:-Release}"
OUTPUT_DIR="${2:-./publish}"

echo "Publishing Backend.DesktopHost..."
dotnet publish Backend/Backend.DesktopHost/Backend.DesktopHost.csproj \
	-c "$CONFIGURATION" \
	-o "$OUTPUT_DIR"

echo "Publish complete. Output saved to: $OUTPUT_DIR"