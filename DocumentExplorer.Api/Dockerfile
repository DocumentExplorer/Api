FROM microsoft/dotnet:2.1.2-aspnetcore-runtime
WORKDIR /dotnetapp
COPY ./bin/output .
ENV ASPNETCORE_URLS http://*:5000
ENV ASPNETCORE_ENVIRONMENT docker
ENTRYPOINT [ "dotnet", "DocumentExplorer.Api.dll" ]
EXPOSE 5000