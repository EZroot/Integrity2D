#!/bin/bash

# Define the root namespace prefix you want to use
NAMESPACE_PREFIX="Integrity"

# --- Main Script Logic ---

echo "Starting recursive namespace assignment..."

# Find all .cs files recursively, excluding the 'bin' and 'obj' directories.
find . -type f -name "*.cs" ! -path "./bin/*" ! -path "./obj/*" | while read CS_FILE; do

    # 1. Determine the directory path of the current file.
    # We use 'dirname' to get the path, and then replace the leading './' with empty string.
    DIR_PATH=$(dirname "$CS_FILE" | sed 's/^\.\///')

    # 2. Extract the last folder name from the path.
    # If DIR_PATH is '.', it means the file is in the root directory.
    # Otherwise, we extract the part after the last '/'.
    if [ "$DIR_PATH" == "." ]; then
        FOLDER_NAME="Root" # Assign a default name for files in the root folder
    else
        # Use awk to split by '/' and get the last field (the folder name)
        FOLDER_NAME=$(echo "$DIR_PATH" | awk -F'/' '{print $NF}')
    fi

    # 3. Capitalize the first letter of the FOLDER_NAME.
    # The 'tr' part converts the rest to lowercase, ensuring proper PascalCase structure.
    # This is important since your folders are lowercase but C# convention is PascalCase.
    PASCAL_NAME=$(echo "$FOLDER_NAME" | awk '{print toupper(substr($0,1,1))tolower(substr($0,2))}')

    # 4. Construct the final namespace string.
    NEW_NAMESPACE="${NAMESPACE_PREFIX}.${PASCAL_NAME}"

    echo "Processing: $CS_FILE -> Namespace: $NEW_NAMESPACE"

    # 5. Use 'sed' to perform the in-place replacement/insertion.
    # The script first tries to replace any existing 'namespace ...' line.
    # If no namespace is found, it inserts the new one below the first 'using' statement or at the start of the file.

    # Check if a 'namespace' declaration already exists
    if grep -q "namespace" "$CS_FILE"; then
        # Replace existing namespace (including any potential whitespace before it)
        # Note: This handles both 'namespace OldNamespace' and 'namespace { }' block styles.
        sed -i.bak -E "/^[[:space:]]*namespace[[:space:]]+.*[[:space:]]*(\{?) *$/c\
namespace $NEW_NAMESPACE\1" "$CS_FILE"
    else
        # If no namespace exists, insert it after the last 'using' statement (or at the top)

        # 5a. Find the line number of the last 'using' statement.
        LAST_USING=$(grep -n -E '^[[:space:]]*using[[:space:]]+' "$CS_FILE" | tail -1 | cut -d: -f1)

        if [ -n "$LAST_USING" ]; then
            # Insert the new namespace two lines after the last 'using' (for a clean separation)
            INSERT_LINE=$((LAST_USING + 2))
            sed -i.bak "${INSERT_LINE}i\\
namespace $NEW_NAMESPACE
" "$CS_FILE"
        else
            # If no 'using' statements exist, insert it at the beginning of the file (line 1)
            sed -i.bak "1i\\
namespace $NEW_NAMESPACE

" "$CS_FILE"
        fi
    fi

done

echo "---"
echo "Namespace assignment complete."
echo "Backup files (with the .bak extension) have been created for all modified files."
echo "Please review the changes before deploying."