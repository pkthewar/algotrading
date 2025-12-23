import { useEffect, useState } from 'react'
import { api } from '../services/api'

type HealthStatus ={
    Status: string,
    KillSwitch: boolean,
    MarketFeed: boolean,
    Broker: boolean,
    Issues: string[],
    //StrategyEngine
    ServerTimeUtc: Date
}

export default function SystemHealth() {
    const[healthStatus, setHealthStatus] = useState<HealthStatus | null>(null)

    useEffect(() => {
        api.get(`/system/getHealth`).then((res) => {
            setHealthStatus(res.data.healthStatus)
        })
    }, [])

    if(!healthStatus)
        return <div>Loading system health...</div>

    return (
        <div>
            <h2>System Health:</h2>
            <p>Status: {healthStatus.Status}</p>
            <p>KillSwitch: {healthStatus.KillSwitch}</p>
            <p>MarketFeed: {healthStatus.MarketFeed}</p>
            <p>Broker: {healthStatus.Broker}</p>
            {/* <p>StrategyEngine: {healthStatus.StrategyEngine}</p> */}
            
            <h3>Issues</h3>
            {healthStatus.Issues.length > 0 ? (
                <ul>
                    {healthStatus.Issues.map((issue, index) => (
                        <li key = {index}> {issue}</li>
                    ))}
                </ul>
                ): (
                    <p>No issues detected! ✅</p>
                )}

            <p>Server Time UTC: {healthStatus.ServerTimeUtc.toString()}</p>
        </div>
    )
}