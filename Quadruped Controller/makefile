.PHONY: build clean server set-release set-debug

BUILD_TYPE ?= Debug
BUILD_DIR := build/$(BUILD_TYPE)

server: build
	@echo "Running server in $(BUILD_TYPE) mode..."
	./$(BUILD_DIR)/server/Server.exe

# Create the build directory, generate build files, and build the project
build:
	@echo "Building in $(BUILD_TYPE) mode..."
ifeq ($(OS),Windows_NT) # Windows platform
	@if not exist "$(BUILD_DIR)" mkdir "$(BUILD_DIR)"
	@cmake -B $(BUILD_DIR) -G "MinGW Makefiles" -DCMAKE_BUILD_TYPE=$(BUILD_TYPE) -DCMAKE_EXPORT_COMPILE_COMMANDS=ON
	@cmake --build $(BUILD_DIR)
else # Unix/Linux/MacOS
	@mkdir -p $(BUILD_DIR)
	@cmake -B $(BUILD_DIR) -G "Unix Makefiles" -DCMAKE_BUILD_TYPE=$(BUILD_TYPE) -DCMAKE_EXPORT_COMPILE_COMMANDS=ON
	@cmake --build $(BUILD_DIR)
endif

# Clean the build directory
clean:
	@echo "Cleaning build directory..."
ifeq ($(OS),Windows_NT)
	@if exist "$(BUILD_DIR)" (rmdir /s /q "$(BUILD_DIR)") else (echo "No build directory to clean.")
else
	@if [ -d "$(BUILD_DIR)" ]; then \
		rm -rf $(BUILD_DIR); \
		echo "Cleaned $(BUILD_TYPE) build directory."; \
	else \
		echo "No build directory to clean."; \
	fi
endif

# Helper target for switching build types
set-release:
	$(eval BUILD_TYPE := Release)

set-debug:
	$(eval BUILD_TYPE := Debug)
