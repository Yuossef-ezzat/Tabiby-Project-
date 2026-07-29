import * as signalR from '@microsoft/signalr'
import { API_BASE_URL, getToken } from './client'


const HUB_ROOT = API_BASE_URL.replace(/\/api\/?$/, '')

function buildConnection(hubPath) {
  return new signalR.HubConnectionBuilder()
    .withUrl(`${HUB_ROOT}${hubPath}`, {
      accessTokenFactory: () => getToken() || '',
    })
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.Warning)
    .build()
}

export function createChatConnection() {
  return buildConnection('/chatHub')
}

export function createNotificationConnection() {
  return buildConnection('/notificationHub')
}
