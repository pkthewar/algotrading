import * as signalR from '@microsoft/signalr'
import { backendUrls } from './api'

export const hubConnection = new signalR.HubConnectionBuilder()
  .withUrl(backendUrls.hubUrl)
  .withAutomaticReconnect()
  .build()
