# -app-port ��Ҫ��Ӧ�������˿�һ��

# -dapr-http-port �� -dapr-grpc-port ��Ҫ��Ӧ������һ�£������磺
#  "Dapr": {
#    "DaprHttpPort": 35100,
#    "DaprGrpcPort": 50100
#  }

daprd -app-id=minijob -config="config.yaml" -components-path="components" -app-ssl=true -enable-metrics=false -placement-host-address=localhost:6050 -dapr-http-port=35100 -dapr-grpc-port=50100 -app-port=44328