.PHONY: build clean server

BUILD_TYPE ?= Debug
BUILD_DIR := build/$(BUILD_TYPE)

build:
	@if [ "$(BUILD_TYPE)" = "Release" ]; then \
		echo "Building in Release mode with -O3 optimization..."; \
		cmake -B $(BUILD_DIR) -G "MinGW Makefiles" -DCMAKE_BUILD_TYPE=Release -DCMAKE_CXX_FLAGS_RELEASE="-O3" -DCMAKE_EXPORT_COMPILE_COMMANDS=ON; \
	else \
		echo "Building in Debug mode..."; \
		cmake -B $(BUILD_DIR) -G "MinGW Makefiles" -DCMAKE_BUILD_TYPE=Debug -DCMAKE_EXPORT_COMPILE_COMMANDS=ON; \
	fi
	cmake --build $(BUILD_DIR) --target Server

clean:
	@if [ -d "$(BUILD_DIR)" ]; then \
		echo "Cleaning $(BUILD_TYPE) build directory..."; \
		rm -rf $(BUILD_DIR); \
	else \
		echo "No build directory for $(BUILD_TYPE) exists to clean."; \
	fi

server: build
	@echo "Running server in $(BUILD_TYPE) mode..."
	./$(BUILD_DIR)/server/Server.exe
