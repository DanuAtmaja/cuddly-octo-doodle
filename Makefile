# Project Variables
PROJECT_NAME ?= SenjaCoffee
ORG_NAME ?= SenjaCoffee
REPO_NAME ?= SenjaCoffee

.PHONY: migrations db

migrations:
	cd ./SenjaCoffee.Data && dotnet ef --startup-project ../SenjaCoffee.Web/ migrations add $(mname) && cd ..

db :
	cd ./SolarCoffee.Data && dotnet ef --startup-project ../SenjaCoffee.Web/ database update && cd ..
