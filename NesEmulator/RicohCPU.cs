using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NesEmulator
{
/*
 * http://nesdev.com/6502.txt
ADC Add with carry. Adds A plus another number, plus the Carry flag, and
	stores the result in A. The other number (source) can be a constant
	(ADC #45), an address (ADC 100h) or an indexed address (ADC 100h,X).
	Since C is added too, you must clear it before with CLC (except when
	you do 2 byte additions).

AND Logical AND. Does bitwise AND with the accumulator and a source (memory
	or inmediate). Logical AND means that result will be true only if the
	two values are true. In other words, the resultant bit will be 1 only
	if both bits are 1; otherwise it will be 0. For example:
		lda #10101010b
		and #00001111b
	stores	     00001010b in A. As you can see, a "mask" can be used to
	force zeros and let remain what you want.This is useful	when using
	a single byte for several purposes.

ASL Arithmethic Shift Left. Shifts bits to left in A or memory. All bits will
	be moved one position to the left. The rightmost position will be
	filled with a zero, and the leftmost will be stored on the Carry Flag.
	Example:
		lda #10011001b
		asl A
	gives:       00110010b, and C will be set. One of its uses is quick
	multiplication by 2 (as shifting decimal numbers to left multiplies
	by 10).

BCC Branch if Carry Clear. Checks the Carry Flag and, if it is set, branches
	to a certain point; otherwise continues down. Branches are used to
	make decisions: if certain flag is set (or clear), the program will
	continue in another point; if not, the program continues as if
	nothing happened. The point where the program (more exactly, the
	Program Counter) will jump is a number with a range of -127 to +127
	bytes of the actual position. Usually you will use a label to mark
	the place where it will jump. An example is some lines below.

BCS Branch if Carry Set. The same, but when C is set.

BEQ Branch if Equal. Branches when the Zero Flag is set. In other words, when
	the operand is zero. It's called "Equal" because, when comparing, Z
	is set if both terms are equal. Example:
		lda bullet_column
		cmp monster_column
		beq monster_dies
	(here code for monster moves)
	monster_dies
		(die sequence: it disappears and the score increases)
	Here, monster_column and bullet_column are variables, stored
	somewhere in memory. But instead of using the address number, we can
	tell first the assembler we will use labels for them. monster_dies is
	a label	for a position where the program will continue.

	In fact, the most common use of BEQ is checking if something is zero.
	For example:
		lda hero_lives
		beq gameover
		(here code for continue playing)
	gameover
		(continue? insert coins)
	It's useful with AND to check status of individual bits.
	This is an example where we suppose the status of button B is in
	bit 1, in a variable called "joypad1":
		lda joypad1
		and #00000010b	;this blows out everything, except B button
		beq B_not_pressed
		(code for fire ball)
	B_not_pressed (more code)

BIT Bit test. Is like AND, but without storing the result in A.

BMI Branch if Minus. Branches when N is set. This means, when the bit 7	is
	set. Is called "minus" because comparisons (which are actually
	substractions) give a negative number when the second term is greater
	than the first.	Negative numbers have the bit 7 set.
	It is useful too to check quickly bit 7.

BNE Branch if Not Equal. Branches when Z is clear (the operand is not 0).
	The contrary of BEQ.

BPL Branch if Plus. Branches when the Negative Flag is clear, that is, when
	the bit 7 of the operand is clear. One of its most common uses is
	to wait for Vblank:
	wait_for_vblank
		lda 2002h	;bit 7 at 2002h is automatically set to 1
				;when Vblank occurs
		bpl wait_for_vblank
		(code for graphics here)

BRK Break. This makes an interruption. Somewhere says that this instruction
	returns one byte after the correct one. Anyway it is rarely used.

BVC Branch if Overflow Clear. Not very useful.

BVS Branch if Overflow Set. Same.

CLC Clear Carry flag. C becomes 0. You must use it before ADC in most cases.
	Used too before rotations to prevent errors.

CLD Clear Decimal mode. The processor is supposed to begin in Binary mode,
	and you will never use Decimal mode, so you can forget this one.
	Nevertheless I have seen is common tu use it at the beginning.

CLI Clear Interrupt disable. Allows interrupts (NMI not included).
	Seldom used.

CLV Clear Overflow flag. Seldom used.

CMP Compare. Compares A with source, for later use of BEQ, BNE, BMI or BPL.
	The comparison is a substraction that does not give result, but
	affects the flags: if both terms are equal, the result is 0; if the
	first is greater, the result is positive; if the second is greater,
	the result is negative.

CPX Compare X. Compares X with source.

CPY Compare Y. Same.

DEC Decrement source. A memory address is decreased 1.

DEX Decrement X. Commonly used in loops:
		ldx 5	;loop 5 times
	loop5	(code)
		dex
		bne loop5
	One useful loop is addressing a block of memory (where a table with
	data can be stored), or clearing it:
		ldx 50h		;the block is 50h bytes
		lda #0		;all bytes will be 0
	memclear sta block_address,X
		dex
		bne memclear

DEY Decrement Y. Same.

EOR Exclusive OR. Bitwise exclusive OR with A and source.
	This gives 1 only is the two bits are different; if not,
	the result is 0. Example:
		lda #11001100b
		eor #11110000b
	stores       00111100b in A.

INC Increment source. A memory address is increased 1.

INX Increment X. Especially useful for loops with graphics. Since the
	address at PPU is automatically incremented, we can't use DEX,
	or the data we put there will be inverted:
		lda #20h	;address of name table
		sta 2006h
		lda #0Ch
		sta 2006h
		ldx #0
	loopgfx lda mem_block,X
		sta 2007h
		inx
		cpx #20h	;we are coying 20h bytes
		bne loopgfx
	As you can see, the use of INX involves a CPX instruction, so
	loops use usually DEX.

INY Increment Y. Same.

JMP Jump. The program jumps to a label and continues from there.

JSR Jump to Subroutine. Is a jump too, but the PC is saved in the stack
	before, so when the code of the subroutine ends, with RTS you return
	where the subroutine was "called". This makes possible to call the
	same subroutine from many points in the program. But often the
	subroutine will need parameters for its actions. They can be passed
	in the registers, memory, or manipulating the stack.
	Look this example, which uses the registers (the easiest way):
		ldx #5		;X will be the enemies number (5 enemies)
	loop_enemies
		jsr enemy_actions
		dex
		bne loop_enemies
		(rest of program)

	enemy_actions
		(here code to approach hero, throw knifes, etc)
		rts

	With this you can control 5 enemies with the same code.

LDA Load Accumulator. Stores in A the value of a constant or a memory address.
	Can be used this way:
		lda #7	This stores in A the value 7
		lda 7	This stores in A the value in the memory address 7
		lda 7,X	This stores in A the value in the memory address plus
	X. This means that if X is 0, A will have the value in 7; if X is 5,
	A will have the value of the memory address 12 (0Bh).

LDX Load X. The same, but obviously indexed addresing can't be used.

LDY Load Y. Same.

LSR Logical Shift Right. Shifts to right A or a memory address. The leftmost
	bit is filled with 0 and the rightmost goes to C. Example:
		lda #01100110b
		lsr A
	stores       00110011b in A, and the Carry Flag will be clear.
	One of its uses is for quick divisions by 2.

NOP No Operation. This does nothing. Can be used for delaying loops
	(sometimes we need less speed), but in that cases is preferable
	to use a slower operation, as this is one of the fastests.

ORA Logical Inclusive OR. Bitwise OR with A and a source. The result will be
	false only if both bits are 0; otherwise it will be 1. Example:
		lda #11001100b
		ora #10101010b
	stores       11101110b in A. This can be used to force bits to 1
	(as AND is used to force 0). For example, suppose we have the
	variable hero_status to indicate if he is jumping with the bit 5,
	and if he is dead with the bit 2:
		(code to decide when he'll die)
		lda hero_status
		ora #00000100b		;set die bit
		sta hero_status
	Now the jump:
		(code to know if button A is pressed)
		lda hero_status
		ora #00100000b		;we force bit 5 to 1, without
		sta hero_status		;touching the other bits
		(code to jump)
	Now, suppose he has finished his jump:
		(code to know when finish jumping)
		lda hero_status
		and #11011111b		;force bit 5 to 0, without
		sta hero_status		;touching other bits
	As you can see, with ORA, 0 lefts bits unchanged and 1 forces 1; with
	AND, 1 lefts bits unchanged and 0 forces 0.

PHA Push accumulator. Stores A in the stack.

PHP Push the Processor Status Register in the stack. Stores P (aka flags) in
	the stack.

PLA Pull Accumulator. Stores in A the value in the top of the stack.

PLP Pull Processor Status Register.  Stores in the flags the value in the
	top of the stack.

ROL Rotate Left. Rotates the bits in A or source one position to the left.
	The rightmost bit is filled with the value of the Carry Flag, and
	the left most bit is sent to the Carry Flag. Example:
		clc		;C will be 0
		lda #10010111b
		rol A
	(A is now    00101110b, and C is set to 1)
		rol A
	(A is now    01011101b, and C is 0)
		rol A
	(A is now    10111010b and C is 0)
		rol A
	(A is now    01110100b and C is 1)
	This is useful to fill a byte when a flow of one bit cames from
	somehwhere. Especifically, from the joypads.

RTI Return from Interrupt. After the interrupt code is executed, this
	returns to where the program was left restoring the PC and the
	flags that were pushed on the stack when the interrupt began.

RTS Return from Subroutine. This returns from a subroutine to the next
	instruction to where it was called, pulling PC from the stack.

SBC Substract with Carry. Stores in A the result of substracting source
	from A. In a book there was written that the value of C is
	irrelevant here, but anyway is preferable clear it before.

SEC Set Carry flag. Sets C to 1.

SED Set Decimal mode. You won't need this.

SEI Set Interrupt disable. When the Interrup t disable flag is set, the
	processor ignores interrupt signals. NMI are not included, they
	will continue working. As far as I know, the only interrupt in
	the NES is one that executes every Vblank, but this is a Non
	Maskable Interrupt (NMI), which can't be affected by this flag.
	It is enabled or disabled with a register in 2000h.
	Nevertheless seems is common to use this at the beginning.

STA Store Accumulator. Stores A in memory. Indexed addressing can be used.

STX Store X. Stores X in memory.

STY Store Y. Same.

TAX Transfer A to X. Stores A in X. Store instructions don't allows transfer
	of data between registers, so the existence of the Transfer ones.

TAY Transfer A to Y.

TSX Transfer S to X. Stores in X the value of the Stack pointer, to know
	where it is.

TXA Transfer X to A.

TXS Transfer X to S. Stores in S the value of X. This must be used always
	at the beginning of a program to use the stack, because it doesn't
	begin to work automatically. If you forget this, you won't be able
	to use subroutines nor interrupts. This way:
		ldx #0FFh
		txs
	This instruction can be used too to manipulate the stack, but that
	is quite complex.

TYX Transfer Y to A.
*/
    public class RicohCPU
    {
        public enum OpCodeImmediate : ushort
        {
            ADC = 0x69,
            AND = 0x29,
            ASL = 0x0A,
            BCC = 0x90
        }

        public enum Flags : ushort
        {
            Carry = 1 << 0,
            Zero = 1 << 1,
            InterruptDisable = 1 << 2,
            Decimal = 1 << 3,
            Break = 1 << 4,
            AlwaysSet = 1 << 5,
            Overflow = 1 << 6,
            Negative = 1 << 7
        }

        public enum AddressingMode
        {
            Immediate
        }

        protected ushort m_pc = 0;         // program counter
        protected ushort m_stack = 0;      // stack register  
        protected ushort m_flagReg = 0;    // flag register - Also called P
        protected ushort m_regA = 0;       // accumulator
        protected ushort m_regX = 0;       // Index Register X
        protected ushort m_regY = 0;       // Index Register Y

        private AddressingMode m_mode = AddressingMode.Immediate;   // addressing mode
        private MainMemory m_memory;     

        public RicohCPU()
        {

        }

        public void Init(MainMemory InMemory)
        {
            m_memory = InMemory;
            m_flagReg = 0x34;    // irq disabled
            m_stack = 0xfd;
            m_regA = m_regX = m_regY = 0;
        }

        public void Eval(Operation op)
        {
            switch(m_mode)
            {
                case AddressingMode.Immediate:
                    EvalImmediate(op);
                    break;
                default:
                    throw new System.InvalidOperationException("Addressing Mode not supported:"+m_mode);
            }
        }

        private void AddAccumulator(ushort value, bool isCarry)
        {
            uint addValue = (uint)value + (uint)m_regA;
            if (isCarry)
            {
                if(addValue > short.MaxValue)
                {
                    m_flagReg = (ushort)(m_flagReg | (ushort)Flags.Carry);
                }
            }
            m_regA += (ushort)addValue;
        }

        // immediate mode 
        public void EvalImmediate(Operation op) 
        {
            switch(op.m_op)
            {
                case (ushort)OpCodeImmediate.ADC:
                    AddAccumulator(m_memory.Read(op.m_arg1), true);
                    break;
                default:
                    throw new System.InvalidOperationException("In Mode "+m_mode+" OpCode not supported: " + op);
            }
        }

    }
}
