import { execSync } from 'child_process';
import { existsSync } from 'fs';
import { resolve } from 'path';

/**
 * Script to generate API client from OpenAPI specification using Orval
 * This script validates the swagger.json file exists and executes Orval generation
 */

const SWAGGER_PATH = resolve(process.cwd(), 'swagger.json');
const CONFIG_PATH = resolve(process.cwd(), 'orval.config.js');

function validateFiles(): void {
    console.log('🔍 Validating required files...');

    if (!existsSync(SWAGGER_PATH)) {
        console.error('❌ Error: swagger.json not found in project root');
        console.error('   Please ensure swagger.json exists before running generation');
        process.exit(1);
    }

    if (!existsSync(CONFIG_PATH)) {
        console.error('❌ Error: orval.config.js not found in project root');
        console.error('   Please ensure orval.config.js exists before running generation');
        process.exit(1);
    }

    console.log('✅ All required files found');
}

function generateApiClient(): void {
    try {
        console.log('🚀 Starting API client generation...');
        console.log(`   Input: ${SWAGGER_PATH}`);
        console.log(`   Config: ${CONFIG_PATH}`);
        console.log('');

        // Execute Orval generation
        execSync('orval --config orval.config.js', {
            stdio: 'inherit',
            cwd: process.cwd(),
        });

        console.log('');
        console.log('✅ API client generated successfully!');
        console.log('   Generated files: ./api/generated-orval/');
        console.log('   Generated schemas: ./api/generated-orval/schemas/');
    } catch (error) {
        console.error('❌ Error during API client generation:');
        if (error instanceof Error) {
            console.error(`   ${error.message}`);
        }
        process.exit(1);
    }
}

function main(): void {
    console.log('═══════════════════════════════════════════════════════');
    console.log('  Orval API Client Generator');
    console.log('═══════════════════════════════════════════════════════');
    console.log('');

    try {
        validateFiles();
        console.log('');
        generateApiClient();
        console.log('');
        console.log('═══════════════════════════════════════════════════════');
        console.log('  Generation Complete!');
        console.log('═══════════════════════════════════════════════════════');
    } catch (error) {
        console.error('');
        console.error('═══════════════════════════════════════════════════════');
        console.error('  Generation Failed');
        console.error('═══════════════════════════════════════════════════════');
        process.exit(1);
    }
}

main();
