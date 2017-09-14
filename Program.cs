using System;

namespace ArkeLogic {
    public class Program {
        public static void Main() {
            var a = new ExternalInput();
            var b = new ExternalInput();

            var gate = new NxorGate(a, b);

            while (true) {
                var aa = Console.ReadKey();
                Console.Write(" ");
                var bb = Console.ReadKey();
                Console.Write(" ");

                a.Value = aa.KeyChar == '1';
                b.Value = bb.KeyChar == '1';

                gate.Tick();

                Console.WriteLine(gate.Value ? "1" : "0");
            }
        }
    }

    public interface ILine {
        bool Value { get; }
    }

    public sealed class ExternalInput : ILine {
        public bool Value { get; set; }
    }

    public abstract class IC {
        public abstract void Tick();
    }

    public abstract class Gate : IC, ILine {
        public bool Value { get; private set; }

        public override void Tick() => this.Value = this.OnTick();

        protected abstract bool OnTick();
    }

    public class NotGate : Gate {
        public ILine I { get; }

        public NotGate(ILine i) => this.I = i;

        protected override bool OnTick() => !this.I.Value;
    }

    public class AndGate : Gate {
        public ILine A { get; }
        public ILine B { get; }

        public AndGate(ILine a, ILine b) => (this.A, this.B) = (a, b);

        protected override bool OnTick() => this.A.Value && this.B.Value;
    }

    public class OrGate : Gate {
        public ILine A { get; }
        public ILine B { get; }

        public OrGate(ILine a, ILine b) => (this.A, this.B) = (a, b);

        protected override bool OnTick() => this.A.Value || this.B.Value;
    }

    public class XorGate : Gate {
        public ILine A { get; }
        public ILine B { get; }

        public XorGate(ILine a, ILine b) => (this.A, this.B) = (a, b);

        protected override bool OnTick() => this.A.Value ^ this.B.Value;
    }

    public class NandGate : Gate {
        private readonly AndGate andGate;
        private readonly NotGate notGate;

        public ILine A { get; }
        public ILine B { get; }

        public NandGate(ILine a, ILine b) {
            (this.A, this.B) = (a, b);

            this.andGate = new AndGate(this.A, this.B);
            this.notGate = new NotGate(this.andGate);
        }

        protected override bool OnTick() {
            this.andGate.Tick();
            this.notGate.Tick();

            return this.notGate.Value;
        }
    }

    public class NorGate : Gate {
        private readonly OrGate orGate;
        private readonly NotGate notGate;

        public ILine A { get; }
        public ILine B { get; }

        public NorGate(ILine a, ILine b) {
            (this.A, this.B) = (a, b);

            this.orGate = new OrGate(this.A, this.B);
            this.notGate = new NotGate(this.orGate);
        }

        protected override bool OnTick() {
            this.orGate.Tick();
            this.notGate.Tick();

            return this.notGate.Value;
        }
    }

    public class NxorGate : Gate {
        private readonly XorGate xorGate;
        private readonly NotGate notGate;

        public ILine A { get; }
        public ILine B { get; }

        public NxorGate(ILine a, ILine b) {
            (this.A, this.B) = (a, b);

            this.xorGate = new XorGate(this.A, this.B);
            this.notGate = new NotGate(this.xorGate);
        }

        protected override bool OnTick() {
            this.xorGate.Tick();
            this.notGate.Tick();

            return this.notGate.Value;
        }
    }
}
