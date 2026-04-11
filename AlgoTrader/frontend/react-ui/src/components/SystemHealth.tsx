import { useEffect, useState } from 'react'
import { api } from '../services/api'
import type { SystemHealthStatusDto } from '../types/api'

export default function SystemHealth() {
  const [healthStatus, setHealthStatus] = useState<SystemHealthStatusDto | null>(
    null,
  )

  useEffect(() => {
    api.get<SystemHealthStatusDto>('/system/getHealth').then((res) => {
      setHealthStatus(res.data)
    })
  }, [])

  if (!healthStatus) {
    return <div>Loading system health...</div>
  }

  return (
    <div>
      <h2>System Health</h2>

      <ul>
        <li>Overall Status: {healthStatus.overallStatus}</li>
        <li>Kill Switch Triggered: {String(healthStatus.killSwitchTriggered)}</li>
        <li>Market Feed Connected: {String(healthStatus.marketFeedConnected)}</li>
        <li>Broker Connected: {String(healthStatus.brokerConnected)}</li>
        <li>
          Strategy Engine Running: {String(healthStatus.strategyEngineRunning)}
        </li>
        <li>System Latency (ms): {healthStatus.systemLatencyMs}</li>
        <li>
          Server Time UTC:{' '}
          {new Date(healthStatus.serverTimeUtc).toLocaleString('en-IN', {
            hour12: false,
          })}
        </li>
      </ul>

      {healthStatus.issues.length > 0 ? (
        <ul>
          {healthStatus.issues.map((issue) => (
            <li key={issue}>{issue}</li>
          ))}
        </ul>
      ) : (
        <p>No issues detected.</p>
      )}
    </div>
  )
}
