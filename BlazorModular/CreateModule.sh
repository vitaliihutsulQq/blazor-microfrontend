#!/bin/bash

MODULE_NAME=$1
MODULE_PATH="Modules/$MODULE_NAME"

if [ -z "$MODULE_NAME" ]; then
	echo "Error: Set the name of module!"
	exit 1

fi

echo "Creating new module $MODULE_NAME..."
dotnet new razorclasslib -o $MODULE_PATH
dotnet sln add $MODULE_PATH/$MODULE_NAME.csproj

echo "Copying DLL in wwwroot/dynamic..."
cp $MODULE_PATH/bin/Release/net8.0/$MODULE_NAME.dll BlazorHost/wwwroot/dynamic/

echo "✅ Module $MODULE_NAME succefull created!"