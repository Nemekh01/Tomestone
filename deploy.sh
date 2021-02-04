aws ecr get-login-password --region us-east-2 | docker login --username AWS --password-stdin 777714646197.dkr.ecr.us-east-2.amazonaws.com
docker build -t lumina .
docker tag lumina:latest 777714646197.dkr.ecr.us-east-2.amazonaws.com/lumina:latest
docker push 777714646197.dkr.ecr.us-east-2.amazonaws.com/lumina:latest