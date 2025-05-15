module.exports = {
    devServer: {
      client: {
        overlay: {
          runtimeErrors: error => {
            const ignoreErrors = [
              'ResizeObserver loop completed with undelivered notifications.'
            ]
            return !ignoreErrors.includes(error.message)
          }
        }
      },
      proxy: {
        '/api': {
          target: 'http://localhost:5129', 
          changeOrigin: true,
        },
      },
    },
  };
  