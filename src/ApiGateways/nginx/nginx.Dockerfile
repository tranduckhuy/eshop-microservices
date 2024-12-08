FROM nginx

COPY src/ApiGateways/nginx/nginx.local.conf /etc/nginx/nginx.conf
COPY src/ApiGateways/nginx/id-local.crt /etc/ssl/certs/id-local.eshop.com.crt
COPY src/ApiGateways/nginx/id-local.key /etc/ssl/private/id-local.eshop.com.key