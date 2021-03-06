﻿using System;

namespace ArkeLogic {
    public class Program {
        public static void Main() {
            var a = new Line();
            var b = new Line();
            var c = new Line();

            var gate = new FullAdder(a, b, c);

            while (true) {
                var aa = Console.ReadKey();
                Console.Write(" ");
                var bb = Console.ReadKey();
                Console.Write(" ");
                var cc = Console.ReadKey();
                Console.Write(" ");

                a.Value = aa.KeyChar == '1';
                b.Value = bb.KeyChar == '1';
                c.Value = cc.KeyChar == '1';

                gate.Tick();

                Console.WriteLine($"S: {(gate.Carry.Value ? "1" : "0")}, C: {(gate.Sum.Value ? "1" : "0")}");
            }
        }
    }

    public interface ILine {
        bool Value { get; }
    }

    public sealed class Line : ILine {
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
        private readonly ILine i;

        public NotGate(ILine i) => this.i = i;

        protected override bool OnTick() => !this.i.Value;
    }

    public class AndGate : Gate {
        private readonly ILine a;
        private readonly ILine b;

        public AndGate(ILine a, ILine b) => (this.a, this.b) = (a, b);

        protected override bool OnTick() => this.a.Value && this.b.Value;
    }

    public class OrGate : Gate {
        private readonly ILine a;
        private readonly ILine b;

        public OrGate(ILine a, ILine b) => (this.a, this.b) = (a, b);

        protected override bool OnTick() => this.a.Value || this.b.Value;
    }

    public class XorGate : Gate {
        private readonly ILine a;
        private readonly ILine b;

        public XorGate(ILine a, ILine b) => (this.a, this.b) = (a, b);

        protected override bool OnTick() => this.a.Value ^ this.b.Value;
    }

    public class NandGate : Gate {
        private readonly ILine a;
        private readonly ILine b;
        private readonly AndGate andGate;
        private readonly NotGate notGate;

        public NandGate(ILine a, ILine b) {
            (this.a, this.b) = (a, b);

            this.andGate = new AndGate(this.a, this.b);
            this.notGate = new NotGate(this.andGate);
        }

        protected override bool OnTick() {
            this.andGate.Tick();
            this.notGate.Tick();

            return this.notGate.Value;
        }
    }

    public class NorGate : Gate {
        private readonly ILine a;
        private readonly ILine b;
        private readonly OrGate orGate;
        private readonly NotGate notGate;

        public NorGate(ILine a, ILine b) {
            (this.a, this.b) = (a, b);

            this.orGate = new OrGate(this.a, this.b);
            this.notGate = new NotGate(this.orGate);
        }

        protected override bool OnTick() {
            this.orGate.Tick();
            this.notGate.Tick();

            return this.notGate.Value;
        }
    }

    public class NxorGate : Gate {
        private readonly ILine a;
        private readonly ILine b;
        private readonly XorGate xorGate;
        private readonly NotGate notGate;

        public NxorGate(ILine a, ILine b) {
            (this.a, this.b) = (a, b);

            this.xorGate = new XorGate(this.a, this.b);
            this.notGate = new NotGate(this.xorGate);
        }

        protected override bool OnTick() {
            this.xorGate.Tick();
            this.notGate.Tick();

            return this.notGate.Value;
        }
    }

    public class HalfAdder : IC {
        private readonly ILine a;
        private readonly ILine b;
        private readonly Line sum;
        private readonly Line carry;
        private readonly XorGate xorGate;
        private readonly AndGate andGate;

        public ILine Sum => this.xorGate;
        public ILine Carry => this.andGate;

        public HalfAdder(ILine a, ILine b) {
            (this.a, this.b, this.sum, this.carry) = (a, b, new Line(), new Line());

            this.xorGate = new XorGate(this.a, this.b);
            this.andGate = new AndGate(this.a, this.b);
        }

        public override void Tick() {
            this.xorGate.Tick();
            this.andGate.Tick();
        }
    }

    public class FullAdder : IC {
        private readonly ILine a;
        private readonly ILine b;
        private readonly ILine carryIn;
        private readonly Line sum;
        private readonly Line carry;
        private readonly HalfAdder adder1;
        private readonly HalfAdder adder2;
        private readonly OrGate orGate;

        public ILine Sum => this.adder2.Sum;
        public ILine Carry => this.orGate;

        public FullAdder(ILine a, ILine b, ILine carryIn) {
            (this.a, this.b, this.carryIn, this.sum, this.carry) = (a, b, carryIn, new Line(), new Line());

            this.adder1 = new HalfAdder(this.a, this.b);
            this.adder2 = new HalfAdder(this.carryIn, this.adder1.Sum);
            this.orGate = new OrGate(this.adder1.Carry, this.adder2.Carry);
        }

        public override void Tick() {
            this.adder1.Tick();
            this.adder2.Tick();
            this.orGate.Tick();
        }
    }
}
