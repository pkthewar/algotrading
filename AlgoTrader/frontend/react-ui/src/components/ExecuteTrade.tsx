import { useState } from 'react'
import { api } from '../services/api'
import { TradeAction, type TradeRequest, type TradeResult } from '../types/api'

export default function ExecuteTrade() {
  const [result, setResult] = useState<TradeResult | null>(null)

  const sampleTrade: TradeRequest = {
    symbol: 'NIFTY',
    quantity: 50,
    tradeAction: TradeAction.Buy,
    price: 25000,
  }

  const exec = async (tradeRequest: TradeRequest) => {
    const response = await api.post<TradeResult>('/trade/execute', tradeRequest)
    setResult(response.data)
  }

  return (
    <div>
      <h2>Execute Trade</h2>
      <button onClick={() => exec(sampleTrade)}>Execute Sample Trade</button>
      {result ? (
        <p>
          {result.symbol} filled at {result.fillPrice} with PnL {result.pnL}
        </p>
      ) : null}
    </div>
  )
}
