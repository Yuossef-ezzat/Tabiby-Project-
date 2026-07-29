import Logo from '../components/common/Logo'
import VitalDivider from '../components/common/VitalDivider'
import authBg from '../assets/images/auth-bg.jpg'

export default function AuthLayout({ eyebrow, title, subtitle, children, footer }) {
  return (
    <div className="relative min-h-screen w-full overflow-hidden bg-ink-950">
      {}
      <img
        src={authBg}
        alt=""
        className="absolute inset-0 h-full w-full object-cover"
      />
      {}
      <div className="absolute inset-0 bg-gradient-to-br from-ink-950/85 via-ink-950/60 to-ink-900/85" />
      <div className="absolute inset-0 bg-grid-lines bg-[size:44px_44px] opacity-40" />

      {}
      <div className="pointer-events-none absolute -left-24 top-1/3 h-72 w-72 rounded-full bg-vital-500/20 blur-[100px]" />
      <div className="pointer-events-none absolute right-0 bottom-0 h-96 w-96 rounded-full bg-vital-500/10 blur-[120px]" />

      <div className="relative z-10 flex min-h-screen w-full flex-col lg:flex-row">
        {}
        <div className="hidden lg:flex lg:w-1/2 lg:flex-col lg:justify-between lg:p-14 xl:p-16">
          <Logo variant="light" size="lg" />

          <div className="max-w-md animate-fadeUp">
            <p className="mb-3 font-body text-sm font-semibold uppercase tracking-[0.2em] text-vital-400">
              Care that keeps up with you
            </p>
            <h1 className="font-display text-4xl font-bold leading-tight text-white xl:text-5xl">
              Your doctor,
              <br />
              your pharmacy,
              <br />
              one heartbeat away.
            </h1>
            <VitalDivider className="my-6 text-vital-400/70" />
            <p className="text-base leading-relaxed text-slate-300">
              Book trusted specialists, chat with your doctor in real time, and get
              medication delivered — all from a single, secure account.
            </p>
          </div>

          <p className="text-xs text-slate-400">
            © {new Date().getFullYear()} Tabiby Health. All rights reserved.
          </p>
        </div>

        {}
        <div className="flex flex-1 items-center justify-center px-5 py-12 sm:px-8 lg:w-1/2">
          <div className="w-full max-w-md animate-fadeUp">
            <div className="mb-8 flex justify-center lg:hidden">
              <Logo variant="light" size="md" />
            </div>

            <div className="glass-panel rounded-xl2 p-8 shadow-panel sm:p-10">
              {eyebrow && (
                <p className="mb-2 font-body text-xs font-semibold uppercase tracking-[0.2em] text-vital-400">
                  {eyebrow}
                </p>
              )}
              <h2 className="font-display text-2xl font-bold text-white sm:text-3xl">{title}</h2>
              {subtitle && <p className="mt-2 text-sm text-slate-300">{subtitle}</p>}

              <div className="mt-7">{children}</div>
            </div>

            {footer && <div className="mt-6 text-center text-sm text-slate-300">{footer}</div>}
          </div>
        </div>
      </div>
    </div>
  )
}
