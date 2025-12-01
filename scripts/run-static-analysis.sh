#!/bin/bash

###############################################################################
# Plantita - Static Code Analysis Runner
# This script runs all static code analysis tools and generates reports
###############################################################################

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo "==========================================="
echo "  Plantita Static Code Analysis"
echo "==========================================="
echo ""

# Navigate to project root
cd "$(dirname "$0")/.."

# Clean previous build
echo -e "${YELLOW}[1/6] Cleaning previous build...${NC}"
dotnet clean --configuration Release > /dev/null 2>&1
echo -e "${GREEN}✓ Clean completed${NC}"
echo ""

# Restore packages
echo -e "${YELLOW}[2/6] Restoring NuGet packages...${NC}"
dotnet restore
echo -e "${GREEN}✓ Restore completed${NC}"
echo ""

# Build with code analysis
echo -e "${YELLOW}[3/6] Building project with code analysis...${NC}"
dotnet build \
    --configuration Release \
    --no-restore \
    /p:EnforceCodeStyleInBuild=true \
    /p:AnalysisLevel=latest \
    /warnasmessage \
    | tee build-analysis.log

if [ ${PIPESTATUS[0]} -ne 0 ]; then
    echo -e "${RED}✗ Build failed${NC}"
    exit 1
fi
echo -e "${GREEN}✓ Build completed${NC}"
echo ""

# Run analyzers explicitly
echo -e "${YELLOW}[4/6] Running code analyzers...${NC}"
dotnet build \
    --no-restore \
    --no-incremental \
    /p:RunAnalyzersDuringBuild=true \
    | tee analyzers.log

if [ ${PIPESTATUS[0]} -ne 0 ]; then
    echo -e "${RED}✗ Analyzers found critical issues${NC}"
    exit 1
fi
echo -e "${GREEN}✓ Analyzers completed${NC}"
echo ""

# Check for warnings
echo -e "${YELLOW}[5/6] Analyzing build output...${NC}"
WARNING_COUNT=$(grep -c "warning" build-analysis.log || true)
ERROR_COUNT=$(grep -c "error" build-analysis.log || true)

echo "Summary:"
echo "  - Errors: ${ERROR_COUNT}"
echo "  - Warnings: ${WARNING_COUNT}"
echo ""

if [ "$ERROR_COUNT" -gt 0 ]; then
    echo -e "${RED}✗ Build contains errors${NC}"
    exit 1
fi

if [ "$WARNING_COUNT" -gt 0 ]; then
    echo -e "${YELLOW}⚠ Build contains ${WARNING_COUNT} warnings${NC}"
    echo ""
    echo "Warnings found:"
    grep "warning" build-analysis.log || true
    echo ""
fi

# Generate report
echo -e "${YELLOW}[6/6] Generating analysis report...${NC}"
cat > static-analysis-report.txt << EOF
========================================
Plantita - Static Code Analysis Report
========================================
Date: $(date)
Branch: $(git branch --show-current)
Commit: $(git rev-parse --short HEAD)

Build Status: SUCCESS
Errors: ${ERROR_COUNT}
Warnings: ${WARNING_COUNT}

Analyzers Enabled:
- Roslyn Analyzers (.NET)
- StyleCop.Analyzers
- SonarAnalyzer.CSharp
- SecurityCodeScan
- Roslynator
- Meziantou.Analyzer
- AsyncFixer

========================================
EOF

if [ "$WARNING_COUNT" -gt 0 ]; then
    echo "" >> static-analysis-report.txt
    echo "Warnings Details:" >> static-analysis-report.txt
    echo "========================================" >> static-analysis-report.txt
    grep "warning" build-analysis.log >> static-analysis-report.txt || true
fi

echo -e "${GREEN}✓ Report generated: static-analysis-report.txt${NC}"
echo ""

# Final status
echo "==========================================="
if [ "$WARNING_COUNT" -eq 0 ]; then
    echo -e "${GREEN}✓ Static Analysis PASSED${NC}"
    echo -e "${GREEN}  No errors or warnings found!${NC}"
else
    echo -e "${YELLOW}⚠ Static Analysis PASSED WITH WARNINGS${NC}"
    echo -e "${YELLOW}  ${WARNING_COUNT} warnings found${NC}"
    echo -e "  Review static-analysis-report.txt for details"
fi
echo "==========================================="

exit 0
