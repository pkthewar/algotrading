import MarketTicker from '../components/MarketTicker'
import StrategyControl from '../components/StrategyControl'
import Portfolio from '../components/Portfolio'
import KillSwitch from '../components/KillSwitch'
import SystemHealth from '../components/SystemHealth'
import ExecuteTrade from '../components/ExecuteTrade'

export default function Dashboard() {
  return (
    <div style={{ padding: 20 }}>
      <h1>AlgoTrader Dashboard</h1>

      <MarketTicker />
      <StrategyControl />
      <ExecuteTrade />
      <Portfolio />
      <KillSwitch />
      <SystemHealth />
    </div>
  )
}
