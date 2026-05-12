#!/bin/bash
xvfb-run --auto-servernum --server-args='-screen 0 640x480x24:32' /root/build/linux_build.x86_64 -batchmode -nographics