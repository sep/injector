#!/usr/bin/env bash

SCRIPT_ARGUMENTS=()

# Parse arguments.
for i in "$@"; do
    case $1 in
        --) shift; SCRIPT_ARGUMENTS+=("$@"); break ;;
        *) SCRIPT_ARGUMENTS+=("$1") ;;
    esac
    shift
done

powershell -ExecutionPolicy ByPass -File build.ps1 "${SCRIPT_ARGUMENTS[@]}"
