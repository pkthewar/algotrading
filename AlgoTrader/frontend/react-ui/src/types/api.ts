export const TradeAction = {
  Buy: 0,
  Sell: 1,
  Hold: 2,
} as const

export type TradeAction = (typeof TradeAction)[keyof typeof TradeAction]

export type SystemHealthStatusDto = {
  overallStatus: string
  killSwitchTriggered: boolean
  marketFeedConnected: boolean
  brokerConnected: boolean
  strategyEngineRunning: boolean
  systemLatencyMs: number
  serverTimeUtc: string
  issues: string[]
}

export type PositionSnapshot = {
  symbol: string
  quantity: number
  avgPrice: number
}

export type PortfolioSnapshot = {
  cash: number
  positions: Record<string, PositionSnapshot>
}

export type TradeRequest = {
  symbol: string
  quantity: number
  tradeAction: TradeAction
  price: number
}

export type TradeResult = {
  symbol: string
  quantity: number
  fillPrice: number
  success: boolean
  time: string
  pnL: number
}

export type MarketTick = {
  symbol: string
  price: number
  time: string
}
