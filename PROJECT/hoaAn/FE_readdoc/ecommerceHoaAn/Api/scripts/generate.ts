import { generate } from 'openapi-typescript-codegen';
import { execSync } from 'child_process';
import path from 'path';

const swaggerPath = path.resolve('./swagger.json');

// 1) Generate Orval
console.log('🚀 Running ORVAL...');
execSync('npx orval --config orval.config.js', { stdio: 'inherit' });

// 2) Generate openapi-typescript-codegen
console.log('🚀 Running openapi-typescript-codegen...');
generate({
  input: swaggerPath,
  output: './src/Api/generated-client',
  clientName: 'ApiClient',
  useUnionTypes: true,
});

console.log('🎉 All API clients generated successfully!');
