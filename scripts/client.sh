# Unused by VS Code tasks
chmod +x "$(pwd)/scripts/build.sh"
"$(pwd)/scripts/build.sh"
echo "Launching game client..."
dotnet "$VINTAGE_STORY/Vintagestory.dll" --tracelog --addModPath "$(pwd)/Releases"
