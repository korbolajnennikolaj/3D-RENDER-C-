import asyncio

class EchoServer:
    def __init__(self, HOST, PORT):
        self.clients = set()
        self.host = HOST
        self.port = PORT
    async def client_handler(self, reader, writer):
        self.clients.add(writer)
        print(f"Client {writer.get_extra_info('peername')} connected")

        try:
            while True:
                data = await reader.read(1024)
                print(f"Client {writer.get_extra_info('peername')} send {data.decode()}")
                if not data:
                    break

                for client in self.clients:
                    if client != writer:
                        try:
                            client.write(data)
                            await client.drain()
                        except asyncio.CancelledError:
                            pass
                        except:
                            print(f"Client {client.get_extra_info('peername')} disconnected")
                            self.clients.remove(client)

        except asyncio.CancelledError:
            pass
        except:
            print(f"Client {writer.get_extra_info('peername')} disconnected")
            self.clients.remove(writer)

    async def start(self):
        server = await asyncio.start_server(self.client_handler, self.host, self.port)
        async with server:
            print("Server started")
            await server.serve_forever()

async def main():
    server = EchoServer('127.0.0.1', 20032)
    await server.start()

if __name__ == "__main__":
    asyncio.run(main())
