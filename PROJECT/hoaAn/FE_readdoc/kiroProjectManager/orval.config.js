/**
 * Orval Configuration
 * 
 * This configuration file defines how Orval generates TypeScript API clients
 * from OpenAPI specifications.
 * 
 * Configuration Options:
 * - input.target: Path to the OpenAPI/Swagger specification file
 * - output.mode: 'tags-split' generates separate files for each API tag
 * - output.target: Main output file path
 * - output.schemas: Directory for generated TypeScript types/interfaces
 * - output.client: HTTP client library to use (axios)
 * - output.clean: Clean output directory before generation
 * - output.prettier: Format generated code with prettier
 * - override.mutator: Custom Axios instance for all API calls
 * 
 * Usage:
 * - npm run api:generate - Generate API client once
 * - npm run api:watch - Watch for changes and regenerate
 * - npx tsx api/scripts/generate.ts - Run generation script with validation
 */

/**
 * Orval Configuration
 * 
 * This configuration generates:
 * 1. TypeScript types/interfaces from OpenAPI schemas
 * 2. Axios-based API client functions
 * 3. React Query hooks for data fetching and mutations
 * 4. Service layer functions
 * 
 * Output structure:
 * - api/generated-orval/{tag}/{tag}.ts - API client functions
 * - api/generated-orval/{tag}/{tag}.hooks.ts - React Query hooks
 * - api/generated-orval/schemas/ - TypeScript types
 * 
 * Usage:
 * - npm run api:generate - Generate API client
 * - npm run api:watch - Watch for changes
 */
module.exports = {
    'api-client': {
        input: './swagger.json',

        output: {
            mode: 'tags-split',
            target: './api/generated-orval/index.ts',
            schemas: './api/generated-orval/schemas',

            client: 'react-query',
            clean: true,
            prettier: true,

            override: {
                mutator: {
                    path: './src/lib/api/orval-client.ts',
                    name: 'apiClient',
                },
            }
        }
    }
};

