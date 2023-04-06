const { env } = require('process');

const target = env.ASPNETCORE_HTTPS_PORT ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}` :
  env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'http://localhost:25606';

const PROXY_CONFIG = {
  "/api": {
    "target": target,
    "changeOrigin": true,
    "secure": false,
    "logLevel": "debug",
    "headers": {
      "Connection": 'Keep-Alive'
    }
  }
};

module.exports = PROXY_CONFIG;
