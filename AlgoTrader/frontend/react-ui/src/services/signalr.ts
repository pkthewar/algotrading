import * as signalR from '@microsoft/signalr'

export const hubConnection = new signalR.HubConnectionBuilder().withUrl("https://localhost:5001/marketHub").withAutomaticReconnect().build()