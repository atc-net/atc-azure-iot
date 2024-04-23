@ECHO OFF
CLS

ECHO Generating OPCPublisher Certificate

openssl genrsa -out server.pass.key 2048
openssl rsa -in server.pass.key -out server.key

openssl req -new -key server.key -out server.csr -subj "/CN=opcpublisher/O=myorganization"

echo authorityKeyIdentifier= keyid,issuer > cert.ext
echo basicConstraints=CA:FALSE >> cert.ext
echo keyUsage=digitalSignature, nonRepudiation, keyEncipherment, dataEncipherment >> cert.ext
echo extendedKeyUsage=clientAuth >> cert.ext
echo subjectKeyIdentifier=hash >> cert.ext
echo subjectAltName=@alt_names >> cert.ext
echo [alt_names] >> cert.ext
echo URI.1 = urn:localhost:Microsoft.Azure.IIoT:microsoft, >> cert.ext
echo DNS.1 = iotedge >> cert.ext

REM # Cert file must be in der format
openssl x509 -req -sha256 -extfile cert.ext -days 9999 -in server.csr -signkey server.key -out server.der

REM # Key must be in pfx format
openssl pkcs12 -export -keypbe NONE -certpbe NONE -name server -in server.der -inkey server.key -out server.pfx