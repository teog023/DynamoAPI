#!/bin/bash
#dotnet publish -c Release -o API/Dynamo
cd /
sudo yum update -y
sudo rpm -Uvh https://packages.microsoft.com/config/centos/7/packages-microsoft-prod.rpm
sudo yum -y install httpd libunwind libicu
sudo service httpd start
sudo chkconfig httpd on
sudo curl -sSL -o dotnet.tar.gz https://go.microsoft.com/fwlink/?LinkID=835019
sudo mkdir -p /opt/dotnet && sudo tar zxf dotnet.tar.gz -C /opt/dotnet
sudo ln -s /opt/dotnet/dotnet /usr/local/bi
sudo yum -y install dotnet-sdk-3.1
sudo yum install -y gcc-c++ make
sudo yum install -y nodejs
sudo npm install -y -g @angular/cli
sudo bash -c 'cat << \EOF > /etc/httpd/conf.d/DynamoAPI.conf
<Directory "/var/www/html">
    AllowOverride All
    Order allow,deny
    Allow from all
    Header set Access-Control-Allow-Origin "*"
    <IfModule mod_authz_core.c>
        Require all granted
    </IfModule>
</Directory>
<VirtualHost *:*>
    RequestHeader set "X-Forwarded-Proto" expr=%{REQUEST_SCHEME}
</VirtualHost>
<VirtualHost *:80>
    Header always set Access-Control-Allow-Origin "*"
    Header always set Access-Control-Allow-Methods "POST, GET, OPTIONS, DELETE, PUT"
    Header always set Access-Control-Max-Age "3600"
    Header always set Access-Control-Allow-Credentials "true"
    Header always set Access-Control-Allow-Headers "x-requested-with, Content-Type, origin, authorization, accept, client-security-token"
    RewriteEngine On
    RewriteCond %{REQUEST_METHOD} OPTIONS
    RewriteRule ^(.*)$ $1 [R=200,L]
    ProxyPreserveHost On
    ProxyPass /dynamo/ http://localhost:5000/
    ProxyPassReverse /dynamo/ http://localhost:5000/
    ErrorLog /etc/httpd/DynamoAPI-error.log
    CustomLog /etc/httpd/DynamoAPI-access.log common
</VirtualHost>
EOF'
sudo chmod -R 777 /var/www/html/
sudo rm -r -d -f mkdir /DynamoAPI
sudo mkdir /DynamoAPI
sudo wget https://github.com/teog023/DynamoAPI/releases/download/10/DynamoAPI.zip -P /
sudo unzip /DynamoAPI.zip -d /
sudo cp -a /DynamoAPI/. /var/www/html/
sudo cp -R /DynamoAPI /var/www/DynamoAPI
sudo bash -c 'cat << \EOF > /var/www/html/.htaccess
<IfModule mod_rewrite.c>
    RewriteEngine On
    # -- REDIRECTION to https (optional):
    # If you need this, uncomment the next two commands
    # RewriteCond %{HTTPS} !on
    # RewriteRule (.*) https://%{HTTP_HOST}%{REQUEST_URI} [L]
    # --
    RewriteCond %{DOCUMENT_ROOT}%{REQUEST_URI} -f [OR]
    RewriteCond %{DOCUMENT_ROOT}%{REQUEST_URI} -d
    RewriteRule ^.*$ - [NC,L]
    RewriteRule ^(.*) index.html [NC,L]
</IfModule>
EOF'
sudo chmod -R 777 /var/www/html/.htaccess
cd /DynamoAPI/API_RC/DynamoAPI
sudo bash -c 'cat << \EOF > /etc/systemd/system/kestrel-DynamoAPI.service
[Unit]
Description=.NET Web API DynamoAPI DynamoAPI
[Service]
WorkingDirectory=/DynamoAPI/API_RC/DynamoAPI
ExecStart=/usr/bin/dotnet /DynamoAPI/API_RC/DynamoAPI/DynamoAPI.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=dotnet-DynamoAPI
User=root
Environment=ASPNETCORE_ENVIRONMENT=Production 
[Install]
WantedBy=multi-user.target
EOF'
sudo systemctl enable kestrel-DynamoAPI.service
sudo systemctl start kestrel-DynamoAPI.service
sudo systemctl status kestrel-DynamoAPI.service
cd /
sudo systemctl restart httpd