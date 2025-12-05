module.exports = {
  vietCommerce: {
    input: './swagger.json',
    output: {
      target: './Api/generated-orval/index.ts',
      schemas: './Api/generated-orval/schemas',
      client: 'axios',
      baseUrl: true,
      clean: true,
      override: {
        mutator: {
          path: './src/lib/api/orval-client.ts',
          name: 'apiClient'
        },
        fetch: {
          withCredentials: true
        },
        response: true, // ✅ Bắt buộc để Orval generate hàm trả về { data, success, message }
      },
    },
  },
};