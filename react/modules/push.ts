declare const _learnq: any

export default function push(event: any) {
  const learnq = _learnq || []
  learnq.push(event)
}
