/**
 * Clean Swagger JSON
 * 
 * This script cleans up .NET assembly information from schema names in swagger.json
 * to make them compatible with Orval code generation.
 */

import fs from 'fs';
import path from 'path';
import { fileURLToPath } from 'url';

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

const swaggerPath = path.join(__dirname, '../../swagger.json');
const swaggerBackupPath = path.join(__dirname, '../../swagger.backup.json');

console.log('🧹 Cleaning swagger.json...');

// Read swagger file
const swaggerContent = fs.readFileSync(swaggerPath, 'utf8');

// Create backup
fs.writeFileSync(swaggerBackupPath, swaggerContent);
console.log('✅ Backup created: swagger.backup.json');

// Parse JSON
const swagger = JSON.parse(swaggerContent);

// Function to clean schema name
function cleanSchemaName(name) {
    return name
        // Remove assembly version info
        .replace(/, Version=[\d.]+, Culture=\w+, PublicKeyToken=[\w]+/g, '')
        // Remove System.Private.CoreLib references
        .replace(/, System\.Private\.CoreLib/g, '')
        // Remove VietCommerce.Core references  
        .replace(/, VietCommerce\.Core/g, '')
        // Clean up generic type syntax
        .replace(/`\d+\[\[/g, '<')
        .replace(/\]\]/g, '>')
        .replace(/\]\[/g, ', ')
        // Simplify System types
        .replace(/System\.Collections\.Generic\./g, '')
        .replace(/System\./g, '')
        // Clean up VietCommerce namespace
        .replace(/VietCommerce\.Core\.Models\./g, '')
        .replace(/VietCommerce\.Core\.DTOs\./g, '')
        .replace(/VietCommerce\.Core\./g, '');
}

// Clean schema names in components
if (swagger.components && swagger.components.schemas) {
    const oldSchemas = swagger.components.schemas;
    const newSchemas = {};
    const nameMapping = {};

    // Create mapping of old names to new names
    for (const oldName in oldSchemas) {
        const newName = cleanSchemaName(oldName);
        nameMapping[oldName] = newName;
        newSchemas[newName] = oldSchemas[oldName];
    }

    swagger.components.schemas = newSchemas;
    console.log(`✅ Cleaned ${Object.keys(nameMapping).length} schema names`);

    // Update all $ref references throughout the document
    function updateRefs(obj) {
        if (typeof obj !== 'object' || obj === null) return;

        for (const key in obj) {
            if (key === '$ref' && typeof obj[key] === 'string') {
                const refPath = obj[key];
                if (refPath.startsWith('#/components/schemas/')) {
                    const oldSchemaName = refPath.replace('#/components/schemas/', '');
                    const newSchemaName = nameMapping[oldSchemaName];
                    if (newSchemaName) {
                        obj[key] = `#/components/schemas/${newSchemaName}`;
                    }
                }
            } else if (typeof obj[key] === 'object') {
                updateRefs(obj[key]);
            }
        }
    }

    updateRefs(swagger);
    console.log('✅ Updated all $ref references');
}

// Write cleaned swagger
fs.writeFileSync(swaggerPath, JSON.stringify(swagger, null, 2));
console.log('✅ swagger.json cleaned successfully');
console.log('');
console.log('You can now run: npm run api:generate');
