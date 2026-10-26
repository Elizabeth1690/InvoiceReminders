import js from '@eslint/js'
import prettier from 'eslint-config-prettier'
import reactHooks from 'eslint-plugin-react-hooks'
import reactRefresh from 'eslint-plugin-react-refresh'
import simpleImportSort from 'eslint-plugin-simple-import-sort'
import { defineConfig, globalIgnores } from 'eslint/config'
import globals from 'globals'
import tseslint from 'typescript-eslint'

// Compatibles con versiones antiguas y nuevas de los plugins de React.
const reactHooksConfig =
  reactHooks.configs.flat?.recommended ?? reactHooks.configs['recommended-latest']
const reactRefreshConfig = reactRefresh.configs.vite ?? {}

const FEATURES = ['invoices', 'clients', 'reminders']

const relativeImports = {
  group: ['.*'],
  message: 'Usa siempre el alias @/ en lugar de rutas relativas.',
}

const restrictImports = (...patterns) => ['error', { patterns: [relativeImports, ...patterns] }]

// Una feature solo puede usar a otra a través de su index.ts y no puede depender de pages ni de app.
const featureBoundaries = FEATURES.map((feature) => ({
  files: [`src/features/${feature}/**/*.{ts,tsx}`],
  rules: {
    'no-restricted-imports': restrictImports(
      {
        group: FEATURES.filter((other) => other !== feature).map(
          (other) => `@/features/${other}/*`,
        ),
        message: 'Importa otras features únicamente desde su index.ts.',
      },
      {
        group: ['@/pages/*', '@/app/*'],
        message: 'Una feature no puede depender de pages ni de app.',
      },
    ),
  },
}))

export default defineConfig([
  globalIgnores(['dist', 'node_modules']),
  {
    files: ['**/*.{ts,tsx}'],
    extends: [
      js.configs.recommended,
      tseslint.configs.recommended,
      reactHooksConfig,
      reactRefreshConfig,
      prettier,
    ],
    languageOptions: {
      ecmaVersion: 'latest',
      globals: globals.browser,
    },
  },
  {
    files: ['src/**/*.{ts,tsx}'],
    plugins: { 'simple-import-sort': simpleImportSort },
    rules: {
      // Estilo
      'func-style': ['error', 'expression'],
      'no-restricted-syntax': [
        'error',
        {
          selector: 'ExportNamedDeclaration[declaration]',
          message:
            'No uses exports en línea: declara primero y exporta al final con export { ... }.',
        },
        {
          selector:
            'ExportDefaultDeclaration[declaration.type=/^(FunctionDeclaration|ClassDeclaration)$/]',
          message: 'Declara con const y exporta por defecto al final del archivo.',
        },
      ],
      'no-restricted-imports': restrictImports(),
      'simple-import-sort/imports': 'error',
      'simple-import-sort/exports': 'error',

      // Tipado estricto
      '@typescript-eslint/no-explicit-any': 'error',
      '@typescript-eslint/no-non-null-assertion': 'error',
      '@typescript-eslint/consistent-type-imports': 'error',
      '@typescript-eslint/consistent-type-definitions': ['error', 'type'],
    },
  },
  ...featureBoundaries,
  {
    files: ['src/components/ui/**/*.{ts,tsx}'],
    rules: {
      'no-restricted-imports': restrictImports({
        group: ['@/features/*', '@/pages/*', '@/app/*'],
        message: 'Los componentes de ui/ son tontos: no dependen de features, pages ni app.',
      }),
    },
  },
  {
    files: ['src/pages/**/*.{ts,tsx}'],
    rules: {
      'no-restricted-imports': restrictImports({
        group: ['@/features/*/*', '@/app/*'],
        message: 'Importa las features únicamente desde su index.ts.',
      }),
    },
  },
])
