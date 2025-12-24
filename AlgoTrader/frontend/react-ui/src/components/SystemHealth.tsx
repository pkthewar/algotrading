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
    const [health, setHealth] = useState<SystemHealthStatus>()
 
    useEffect(() => {
        api.get('/system/getHealth').then((res) => {
            setHealth(res.data)
        })
    }, [])
    
    return (
        <div>
            <h2>System Health</h2>

            <ul>
                <li>Overall Status: res.data.overallStatus</li>
                <li>Killswitch Triggered: res.data.killSwitchTriggered</li>                
                <li>Market Feed Connected: res.data.marketFeedConnected</li>                
                <li>Broker Connected: res.data.brokerConnected</li>                
                <li>Strategy Engine Running: res.data.strategyEngineRunning</li>                
                <li>System Latency Ms: res.data.systemLatencyMs</li>                
                <li>Server Time Utc: res.data.serverTimeUtc</li>          

                {/* To-Do: Map Issues                                    */}
            </ul>

            {/* <ul>
                {Object.values(res.data.issues)}
            </ul> */}
        </div>
    )

}