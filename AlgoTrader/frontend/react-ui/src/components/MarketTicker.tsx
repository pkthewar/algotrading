import { useEffect, useState } from 'react'
import { hubConnection } from '../services/signalr'
import type { MarketTick } from '../types/api'

export default function MarketTicker() {
  const [price, setPrice] = useState<number>(0)

  useEffect(() => {
    void hubConnection.start().then(() => hubConnection.invoke('Subscribe', 'NIFTY'))

    hubConnection.on('tick', (data: MarketTick) => {
      setPrice(data.price)
    })

    return () => {
      hubConnection.off('tick')
      void hubConnection.invoke('Unsubscribe', 'NIFTY').catch(() => undefined)
    }
  }, [])

  return (
    <div>
      <h2>NIFTY</h2>
      <p>Live Price: {price}</p>
    </div>
  )
}
