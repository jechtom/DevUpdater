You need to enable HTTPS for specific port and server certificate:

netsh http add urlacl url=https://+:25427/ user=Everyone
netsh http add sslcert ipport=0.0.0.0:25427 certhash=dc791029197fb1413478336bccd59dc2e223e42c appid={8a05ce18-c692-45bb-87e0-8642aa3aa8f5}
‎

{8a05ce18-c692-45bb-87e0-8642aa3aa8f5} is selected APP ID