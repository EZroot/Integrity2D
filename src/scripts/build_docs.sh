#!/bin/bash

dotnet build -c Release   

# Resolve the repo root (two levels up from this script)
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/../.." && pwd)"
cd "$REPO_ROOT" || exit

# Ensure docfx exists
if ! dotnet tool list -g | grep -q "docfx"; then
    echo "Installing DocFX..."
    dotnet tool install -g docfx
fi

# Build docs
echo "Building DocFX docs..."
docfx metadata docfx.json
docfx build docfx.json

# Serve _site over HTTP
echo "Serving docs on http://localhost:8000 ..."
cd docs || exit

# Open browser (Linux / macOS)
xdg-open http://localhost:8000/index.html 2>/dev/null || open http://localhost:8000/index.html

# Start simple HTTP server (Python 3)
python3 -m http.server 8000
