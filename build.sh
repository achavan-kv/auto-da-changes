#!/bin/bash
export ReleaseVersion=master
export BUILD_NUMBER=9.0.0.0
../bin/nant/bin/NAnt.exe -buildfile:cosacs.build -D:CCNetRequestSource="" default
# ..\bin\nant\bin\NAnt.exe -buildfile:cosacs.build artifacts-zip
