import { useEffect, useState, type SetStateAction } from 'react'
import { hubConnection } from '../services/signalr'

export default function MarketTicker() {
    const [price, setPrice] = useState<number>(0)

    useEffect(() => {
        hubConnection.start()

        hubConnection.on('tick', (data: { price: SetStateAction<number> }) => {
            setPrice(data.price)
        })

        hubConnection.invoke("Subscribe", "NIFTY")

        return () => {
            hubConnection.off('tick')
        }
    }, [])

    return (
        <div>
            <h2>NIFTY</h2>
            <p>Live Price: {price}</p>
        </div>
    )
}