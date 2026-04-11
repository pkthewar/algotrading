import { api } from '../services/api'

export default function StrategyControl() {
  const start = async (strategy: string) => {
    await api.post<string>(`/strategy/start?strategy=${strategy}`)
  }

  const stop = async () => {
    await api.post<string>('/strategy/stop')
  }

  return (
    <div>
      <h2>Strategy Control</h2>

      <button onClick={() => start('rsi')}>Start RSI</button>
      <button onClick={() => start('macd')}>Start MACD</button>
      <button onClick={stop}>Stop</button>
    </div>
  )
}
