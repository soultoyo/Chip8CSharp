using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Windows.Input;

namespace Chip8_Emulator
{
    class Chip8
    {
        // 4k memory 
        byte[] memory;
        // Registers
        byte[] V = new byte[16];

        ushort program_counter;
        ushort I;
        ushort opcode;

        ushort[] stack;
        char[] keys;
        ushort stack_pointer;
        public bool[,] screen;
        byte[,] sprites;

        bool Running { get; set; }

        // DT = delay timer
        ushort dt = 0;
        // ST = sound timer
        ushort st = 0;

        public bool drawFrame = false;
        
        public Chip8()
        {
            memory = new byte[0xFFF];
            
            V = new byte[16];
            program_counter = 0x200;
            I = 0;
            opcode = 0;
            stack = new ushort[16];
            stack_pointer = 0;
            screen = new bool[64, 32];
            sprites = new byte[16, 5];
            keys = new char[16];

            LoadSprites();

        }

        public void LoadSprites()
        {
            // built-in sprites(font) for the chip-8 system

            // 0 
            sprites[0, 0] = 0xF0;
            sprites[0, 1] = 0x90;
            sprites[0, 2] = 0x90;
            sprites[0, 3] = 0x90;
            sprites[0, 4] = 0xF0;

            // 1
            sprites[1, 0] = 0x20;
            sprites[1, 1] = 0x60;
            sprites[1, 2] = 0x20;
            sprites[1, 3] = 0x20;
            sprites[1, 4] = 0x70;

            // 2
            sprites[2, 0] = 0xF0;
            sprites[2, 1] = 0x10;
            sprites[2, 2] = 0xF0;
            sprites[2, 3] = 0x80;
            sprites[2, 4] = 0xF0;

            // 3
            sprites[3, 0] = 0xF0;
            sprites[3, 1] = 0x10;
            sprites[3, 2] = 0xF0;
            sprites[3, 3] = 0x10;
            sprites[3, 4] = 0xF0;

            // 4
            sprites[4, 0] = 0x90;
            sprites[4, 1] = 0x90;
            sprites[4, 2] = 0xF0;
            sprites[4, 3] = 0x10;
            sprites[4, 4] = 0x10;

            // 5
            sprites[5, 0] = 0xF0;
            sprites[5, 1] = 0x80;
            sprites[5, 2] = 0xF0;
            sprites[5, 3] = 0x10;
            sprites[5, 4] = 0xF0;

            // 6
            sprites[6, 0] = 0xF0;
            sprites[6, 1] = 0x80;
            sprites[6, 2] = 0xF0;
            sprites[6, 3] = 0x90;
            sprites[6, 4] = 0xF0;

            // 7 
            sprites[7, 0] = 0xF0;
            sprites[7, 1] = 0x10;
            sprites[7, 2] = 0x20;
            sprites[7, 3] = 0x40;
            sprites[7, 4] = 0x40;

            // 8 
            sprites[8, 0] = 0xF0;
            sprites[8, 1] = 0x90;
            sprites[8, 2] = 0xF0;
            sprites[8, 3] = 0x90;
            sprites[8, 4] = 0xF0;

            // 9
            sprites[9, 0] = 0xF0;
            sprites[9, 1] = 0x90;
            sprites[9, 2] = 0xF0;
            sprites[9, 3] = 0x10;
            sprites[9, 4] = 0xF0;

            // A
            sprites[10, 0] = 0xF0;
            sprites[10, 1] = 0x90;
            sprites[10, 2] = 0xF0;
            sprites[10, 3] = 0x90;
            sprites[10, 4] = 0x90;

            // B
            sprites[11, 0] = 0xE0;
            sprites[11, 1] = 0x90;
            sprites[11, 2] = 0xE0;
            sprites[11, 3] = 0x90;
            sprites[11, 4] = 0xE0;

            //C
            sprites[12, 0] = 0xF0;
            sprites[12, 1] = 0x80;
            sprites[12, 2] = 0x80;
            sprites[12, 3] = 0x80;
            sprites[12, 4] = 0xF0;

            // D
            sprites[13, 0] = 0xE0;
            sprites[13, 1] = 0x90;
            sprites[13, 2] = 0x90;
            sprites[13, 3] = 0x90;
            sprites[13, 4] = 0xE0;

            // E
            sprites[14, 0] = 0xF0;
            sprites[14, 1] = 0x80;
            sprites[14, 2] = 0xF0;
            sprites[14, 3] = 0x80;
            sprites[14, 4] = 0xF0;

            //F
            sprites[15, 0] = 0xF0;
            sprites[15, 1] = 0x80;
            sprites[15, 2] = 0xF0;
            sprites[15, 3] = 0x80;
            sprites[15, 4] = 0x80;

            int memory_index = 0;
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    memory[memory_index] = sprites[i, j];
                    memory_index++;
                }
            }
        }

        public void LoadProgram(String path)
        {
            byte[] program = File.ReadAllBytes(path);
            ushort program_offset = 0x200;

            for (int i = 0; i < program.Length; i++)
            {
                memory[i + program_offset] = program[i];
                     
            }
        }

        public void SetKeyState(Keys key, char state)
        {
            switch (key)
            {
                case Keys.D1:
                    keys[0x1] = state;
                    break;
                case Keys.D2:
                    keys[0x2] = state;
                    break;
                case Keys.D3:
                    keys[0x3] = state;
                    break;
                case Keys.D4:
                    keys[0xC] = state;
                    break;
                case Keys.Q:
                    keys[0x4] = state;
                    break;
                case Keys.W:
                    keys[0x5] = state;
                    break;
                case Keys.E:
                    keys[0x6] = state;
                    break;
                case Keys.R:
                    keys[0xD] = state;
                    break;
                case Keys.A:
                    keys[0x7] = state;
                    break;
                case Keys.S:
                    keys[0x8] = state;
                    break;
                case Keys.D:
                    keys[0x9] = state;
                    break;
                case Keys.F:
                    keys[0xE] = state;
                    break;
                case Keys.Z:
                    keys[0xA] = state;
                    break;
                case Keys.X:
                    keys[0x0] = state;
                    break;
                case Keys.C:
                    keys[0xB] = state;
                    break;
                case Keys.V:
                    keys[0xF] = state;
                    break;
            }
        }

        public void cycle()
        {
            int VX, VY;

            // The program should end with an error if the program counter is outside of the memory bounds for the loaded program
            // TODO - Need to throw an error exception 
            if(program_counter < 512 || program_counter >= 4096) {
                Console.WriteLine("ERROR: program counter has gone out of bounds");
            }

            // getting the opcode from memory
            // we need to merge the bytes together to get the actual opcode by shifting the current memory over and then using
            // a bitwise or to merge the first byte of data in memory to the second byte.
            opcode = (ushort)((memory[program_counter] << 8) | memory[program_counter + 1]);
            int instruction = opcode & 0xF000;


            // Used CowGod's Chip8 Technical Reference to refer to the opcode/instructions
            // http://devernay.free.fr/hacks/chip8/C8TECH10.HTM
            switch (instruction)
            {
                case 0x0000:
                    if ((ushort)(opcode & 0x00FF) == 0x00E0) // CLS
                    {
                        screen = new bool[64, 32];
                        program_counter += 2;
                    }
                    else if ((ushort)(opcode & 0x00FF) == 0x00EE) // RET
                    {
                        stack_pointer--;
                        program_counter = stack[stack_pointer];
                        program_counter += 2;
                    }
                    break;
                case 0x1000: // JP
                    program_counter = (ushort)(opcode & 0x0FFF);
                    break;
                case 0x2000: // CALL
                    stack[stack_pointer] = program_counter;
                    stack_pointer++;
                    program_counter = (ushort)(opcode & 0x0FFF);
                    break;
                case 0x3000: //3xkk
                    VX = (opcode & 0x0F00) >> 8;
                    if (V[VX] == (ushort)(opcode & 0x00FF))
                    {
                        program_counter += 4;
                    } else
                    {
                        program_counter += 2;
                    }
                    break;
                case 0x4000: //4xkk
                    VX = (opcode & 0x0F00) >> 8;
                    if (V[VX] != (ushort)(opcode & 0x00FF))
                    {
                        program_counter += 4;
                    }
                    else
                    {
                        program_counter += 2;
                    }
                    break;
                case 0x5000: //5xy0
                    VX = (opcode & 0x0F00) >> 8;
                    VY = (opcode & 0x00F0) >> 4;
                    if (V[VX] == V[VY])
                    {
                        program_counter += 4;
                    }
                    else
                    {
                        program_counter += 2;
                    }
                    break;
                case 0x6000: //6xkk
                    VX = (opcode & 0x0F00) >> 8;
                    V[VX] = (byte)(opcode & 0x00FF);
                    program_counter += 2;
                    break;
                case 0x7000: //7xkk
                    VX = (opcode & 0x0F00) >> 8;
                    V[VX] += (byte)(opcode & 0x00FF);
                    program_counter += 2;
                    break;
                case 0x8000:
                    VX = (opcode & 0x0F00) >> 8;
                    VY = (opcode & 0x00F0) >> 4;
                    // We need this additional switch statement for other instructions starting with 0x8000 (Hex)
                    switch (opcode & 0x000F)
                    {
                        case 0x0000: // STORE
                            V[VX] = V[VY];
                            break;
                        case 0x0001: // OR
                            V[VX] = (byte)(V[VX] | V[VY]);
                            break;
                        case 0x0002: // AND
                            V[VX] = (byte)(V[VX] & V[VY]);
                            break;
                        case 0x0003: // XOR
                            V[VX] = (byte)(V[VX] ^ V[VY]);
                            break;
                        case 0x0004: // ADD
                            ushort result = (ushort)(V[VX] + V[VY]);
                            if (result > 255)
                            {
                                V[0xF] = 1;
                            }
                            else
                            {
                                V[0xF] = 0;
                            }
                            V[VX] = (byte)result;
                            break;
                        case 0x0005: // SUB
                            if (V[VX] > V[VY])
                            {
                                V[0xF] = 1;
                            }
                            else
                            {
                                V[0xF] = 0;
                            }
                            V[VX] = (byte)(V[VX] - V[VY]);
                            break;
                        case 0x0006: // SHR
                            if ((ushort)(V[VX] & 1) == 1)
                            {
                                V[0xF] = 1;
                            }
                            else
                            {
                                V[0xF] = 0;
                            }
                            V[VX] = (byte)(V[VY] / 2);
                            break;
                        case 0x0007: // SUBN
                            if (V[VY] > V[VX])
                            {
                                V[0xF] = 1;
                            }
                            else
                            {
                                V[0xF] = 0;
                            }
                            V[VX] = (byte)(V[VX] - V[VY]);
                            break;
                        case 0x00E: // SHL
                            if ((V[VX] >> 7) == 1)
                            {
                                V[0xF] = 1;
                            }
                            else
                            {
                                V[0xF] = 0;
                            }
                            V[VX] *= 2;
                            break;
                    }
                    program_counter += 2;
                    break;
                case 0x9000:
                    VX = (opcode & 0x0F00) >> 8;
                    VY = (opcode & 0x00F0) >> 4;
                    if (V[VX] != V[VY])
                    {
                        program_counter += 4;
                    }
                    else
                    {
                        program_counter += 2;
                    }
                    break;
                case 0xA000: // Set I to the address NNN
                    I = (ushort)(opcode & 0x0FFF);
                    program_counter += 2;
                    break;
                case 0xB000: //JP V0, addr
                    program_counter = (ushort)((opcode & 0xF000) + V[0]);
                    break;
                case 0xC000: //RND Vx, byte
                    Random random = new Random();
                    VX = (opcode & 0x0F00) >> 8;
                    V[VX] = (byte)(random.Next(0, 255) & (opcode & 0xFF00));
                    program_counter += 2;
                    break;
                case 0xD000: //DRW Vx, Vy, nibble
                    VX = (opcode & 0x0F00) >> 8;
                    VY = (opcode & 0x00F0) >> 4;
                    V[0xF] = 0;
                    for (int y = 0; y < (opcode & 0x000F); y++)
                    {
                        byte pixel = memory[I + y];
                        for (int x = 0; x < 8; x++)
                        {
                            if ((pixel & (0x80 >> x)) != 0)
                            { 
                                if (screen[V[VX] + x, V[VY] + y])
                                {
                                    V[0xF] = 1;
                                }
                                screen[V[VX] + x, V[VY] + y] = !screen[V[VX] + x, V[VY] + y];
                            }
                        }
                    }
                    drawFrame = true;
                    program_counter += 2;
                    break;
                case 0xE000: 
                    if ((ushort)(opcode & 0x00FF) == 0x009E) //SKP Vx
                    {
                        VX = (opcode & 0x0F00) >> 8;
                        if (keys[V[VX]] == 1)
                        {
                            program_counter += 4;
                        }
                        else
                        {
                            program_counter += 2;
                        }
                    }
                    else if ((ushort)(opcode & 0x00FF) == 0x00A1) //SKNP Vx
                    {
                        VX = (opcode & 0x0F00) >> 8;
                        if (keys[V[VX]] == 0)
                        {
                            program_counter += 4;
                        }
                        else
                        {
                            program_counter += 2;
                        }
                    }
                    break;
                case 0xF000:
                    VX = (opcode & 0x0F00) >> 8;
                    switch (opcode & 0x00FF)
                    {
                        case 0x0007: //LD Vx, DT
                            V[VX] = (byte)dt;
                            program_counter += 2;
                            break;
                        case 0x000A: //LD Vx, K
                            break;
                        case 0x0015: //LD DT, Vx
                            dt = V[VX];
                            program_counter += 2;
                            break;
                        case 0x018: //LD ST, Vx
                            st = V[VX];
                            program_counter += 2;
                            break;
                        case 0x01E: //ADD I, Vx
                            I += V[VX];
                            program_counter += 2;
                            break;
                        case 0x029: //LD F, Vx
                            I = (ushort)(V[VX] * 5); // I think its 5 because of the offset for a sprite 
                            program_counter += 2;
                            break;
                        case 0x033: //LD B, Vx
                            memory[I] = (byte)(V[VX] / 100);
                            memory[I + 1] = (byte)((V[VX] / 10) % 10);
                            memory[I + 2] = (byte)((V[VX] % 100) % 10);
                            program_counter += 2;
                            break;
                        case 0x055: //LD [I], Vx
                            for (int i = 0; i <= VX; i++)
                            {
                                memory[I + i] = V[i];
                            }
                            program_counter += 2;
                            break;
                        case 0x065: //LD Vx, [I]
                            for (int i = 0; i <= VX; i++)
                            {
                                V[i] = memory[I + i];
                            }
                            program_counter += 2;
                            break;
                    }
                    break;
                default:
                    Console.WriteLine(String.Format("Unknown opcode 0x{0:x4}", opcode));
                    break;
            }

            // Delay timer subtracts 1 at a rate of 60hz
            if (dt > 0)
            {
                dt--;
            }

            // Sound timer subtracts 1 at a rate of 60hz
            // if the sound timer is greater than zero then we need to play a beep
            if (st > 0)
            {
                Console.Beep();
                st--;
            }
        }
    }
}
