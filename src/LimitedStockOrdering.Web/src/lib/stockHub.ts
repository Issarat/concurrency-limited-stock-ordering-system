import {
  HubConnectionBuilder,
  HubConnectionState,
  LogLevel,
  type HubConnection,
} from '@microsoft/signalr'
import { apiBaseUrl } from './api'

let connection: HubConnection | undefined

export function getStockHubConnection(): HubConnection {
  connection ??= new HubConnectionBuilder()
    .withUrl(`${apiBaseUrl}/hubs/stock`)
    .withAutomaticReconnect()
    .configureLogging(LogLevel.Warning)
    .build()

  return connection
}

export async function startStockHub(): Promise<HubConnection> {
  const stockHub = getStockHubConnection()

  if (stockHub.state === HubConnectionState.Disconnected) {
    await stockHub.start()
  }

  return stockHub
}
