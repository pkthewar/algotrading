import { useEffect, useState } from 'react'
import { api } from '../services/api'

type SystemHealthStatus = {
    overallStatus: string,
    killSwitchTriggered: boolean,
    marketFeedConnected: boolean,
    brokerConnected: boolean,
    strategyEngineRunning: boolean,
    systemLatencyMs: number,
    serverTimeUtc: Date,
    issues: Array<string>
}

export default function SystemHealth() {
    const [healthStatus, setHealthStatus] = useState<SystemHealthStatus | null>(null)
 
    useEffect(() => {
        api.get(`/system/getHealth`).then((res) => {
            setHealthStatus(res.data)
        })
    }, [])
    
    if(!healthStatus)
        return (
            <div>Loading System health....</div>
        )

    return (
        <div>
            <h2>System Health</h2>

            <ul>
                <li>Overall Status: {healthStatus?.overallStatus}</li>
                <li>Killswitch Triggered: {healthStatus?.killSwitchTriggered}</li>                
                <li>Market Feed Connected: {healthStatus?.marketFeedConnected}</li>                
                <li>Broker Connected: {healthStatus?.brokerConnected}</li>                
                <li>Strategy Engine Running: {healthStatus?.strategyEngineRunning}</li>                
                <li>System Latency (ms): {healthStatus?.systemLatencyMs}</li>                
                <li>Server Time Utc: {healthStatus?.serverTimeUtc.toString()}</li>          
            </ul>

            {healthStatus.issues.length > 0 ? (
                <ul>
                    {healthStatus.issues.map((issue, index) => (
                        <li key={index}>{issue}</li>
                    ))}
                </ul>
            ) : (
                <p>No issues detected!</p>
            )}
        </div>
    )
}
