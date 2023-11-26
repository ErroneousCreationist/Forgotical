using System;
namespace Forgotical.InternalUtility
{
	public static class Errors
	{
		public static T GetChoice<T>(this T[] Array)
		{
			Random rand = new Random();
			return Array[rand.Next(0, Array.Length)];
		}

		public static Dictionary<string, string[]> ERROR_MESSAGES = new Dictionary<string, string[]>()
		{
			{ "error_noexp", new string[] {
				"Woah there buddy, it seems that the line you have there has... NO OPERATOR?! Are you serious? You really need a : somewhere and an operator! Otherwise it would be really bad and your lovely code wouldn't run! Well it isn't so lovely right now since it clearly isn't running.",
				"Ok now you really expect me to run your code and it has NO OPERATOR?! Like wow are you serious? What am I meant to even do? Sit around and GUESS what your code is meant to do?! Jeez.",
				"No operator man! Like cmon! Its not even that hard to just put an operator with a : dividing between its arguments! Like jeez this is a JOKE language but it isn't THAT hard! Sort of."
			} },
            { "error_badexp", new string[] {
                "SLOW DOWN THERE! That expression just doesn't exist! I don't know what you were thinking, the expressions are rather simple. Its pretty easy to remember them. Even I remember them! Like cmon man you need to step up your game.",
                "Wait a minute... an expression called that doesn't exist... are you trying to trick me? Admit it, you were trying to trick me. Like really man, are you serious? Thats not funny.",
                "Really? Thats not funny. I'm more dissapointed than anything to be honest with you."
            } },
            { "error_badexpstatement", new string[] {
                "Hold up! Are we running a code marathon here? Your line's so packed with expressions, it's like trying to fit an entire circus troupe into a mini cooper! Give those poor expressions some breathing room. Let them shine one at a time!",
                "Whoa, cowboy! Your line's looking like a traffic jam in rush hour. It's expressions galore, jostling for space like commuters in a packed subway. Ease up, let each expression strut its stuff solo!",
                "Hey, speed racer! Your line's a bit like a buffet on a saucer—too much goodness in a cramped space! Give your expressions some legroom. Spread them out, and let them sizzle without stepping on each other's toes!",
				"Really? Thats not funny. I'm more dissapointed than anything to be honest with you."
            } },
            { "error_noargs", new string[] {
                "Ahem! Missing arguments, are we? It's like sending a knight into battle without a trusty sword! I mean, how am I supposed to work miracles here? I'm a compiler, not a mind reader! Toss in those missing arguments, or else this code is just a riddle wrapped in a mystery inside an enigma.",
                "Hold your horses! Your expression's looking a bit lonely without its entourage of arguments! It's like throwing a party and forgetting to invite the guests. Come on, give those expressions the company they deserve! Fill in those arguments, and let's turn this code into a lively shindig!",
                "Whoopsie-daisy! Looks like your expression's feeling a bit underdressed without its arguments! It's like ordering a pizza with no toppings—where's the flavor, the spice? Don't leave your expressions hanging! Tuck in those missing arguments and let's whip up a code masterpiece!",
            } },
            { "error_space", new string[] {
                "Ah, the infamous solo space! Are we trying to add a little minimalist flair to our code, or did it accidentally sneak in? I don't do 'Where's Waldo' with spaces! Let's corral that stray space back to where it belongs—between words, not in the code!",
                "Whoa, Nelly! We've got a renegade space floating in the code! It's like finding a needle in a haystack, only less useful! I'm not here for hide and seek. Let's keep it clean, no rogue spaces allowed!",
                "Hold the phone! A lone wolf space? Seriously? You've got me doing detective work here! I'm not a space ranger, I'm a compiler! How 'bout we keep these spaces in the sentence and not in the code? Tidy it up, partner!",
                "Really? A space? Thats not funny. I'm more dissapointed than anything to be honest with you."
            } },
            { "error_toomanyargs", new string[] {
                "Hold up! Are we trying to throw a banquet for arguments here? Your function's looking like a buffet line at a party where everyone's invited, even the uncles twice removed! I'm all about the celebration, but let's keep it reasonable. Trim down the guest list, please!",
                "Whoa there, maestro! Your function's orchestrating a symphony of arguments! It's like trying to conduct a band with more players than instruments! I'm good at multitasking, but this is pushing it. Let's declutter, streamline those arguments, and keep this code concert manageable.",
                "Ah, the argument fiesta! Your function's turning into a bustling marketplace where everyone's shouting for attention! It's like trying to juggle flaming torches on a unicycle—it's impressive, but let's avoid the chaos. Let's edit that lineup, give those arguments some breathing room, shall we?",
                "Jeez, you are so selfish! So many homeless people with no space to live and are just wasting it willy nilly with your stupid unnessecary arguments? Come on!"
            } },
            { "error_badargs", new string[] {
                "Well, well, well! Look at these arguments trying to crash a party they weren't invited to! It's like bringing a snorkel to a fashion show—completely out of place! I'm no fashion guru, but let's dress this code with the right arguments, shall we? No fashion faux pas in this syntax parade!",
                "Oh, sweet syntax! You're throwing me curveballs with those arguments! It's like asking for a cup of coffee and getting a cup of confetti instead. I'm not a magician; I can't make sense out of nonsense. Toss in the right arguments, and let's make this a meaningful conversation!",
                "Hold your horses! Those arguments are like trying to fit square pegs into round holes! It's a mismatch made in syntax heaven, or should I say, syntax chaos? I'm all about harmony, not this discord! Swap those bad arguments out for the right ones, and let's get this code humming!",
                "Okay, this just makes no sense. Its like you are trying to confuse me here man with these erroneous arguments! Cut it out!"
            } },
            { "error_toomuchmemory", new string[] {
                "Hold up! Are you building a memory palace or launching a rocket to the moon? Your memory request's bigger than a whale in a goldfish bowl! I'm good, but I'm not about to juggle mountains of memory. Let's reel it in, shall we? Keep it reasonable!",
                "Whoa there, memory mogul! Trying to hog all the RAM, are we? It's like ordering the entire menu at a restaurant and expecting it all to fit on one plate! I'm generous, but not with memory. Scale it down, buddy, this buffet's too much for my memory stomach!",
                "Ah, the memory glutton strikes again! It's like trying to stuff an elephant into a suitcase, and let me tell you, that's just not happening! I'm all for ambition, but this is like trying to sip the ocean through a straw. Let's rethink this memory situation, shall we? Moderation is key!",
            } },
            { "error_varmissing", new string[] {
                "Hold your horses! You're trying to summon a unicorn from thin air—a non-existent variable! It's like asking me to pull a rabbit out of an empty hat. I'm good, but not that good! Let's stick to reality and summon variables that actually exist, shall we?",
                "Whoa there, code conjurer! You're waving your wand for a variable that's playing hide-and-seek! It's like expecting me to find Atlantis in the code—it's just not there! I'm a compiler, not a magician. Let's deal with real variables, not the ones hiding in the Bermuda Triangle!",
                "Ah, the quest for the elusive variable! It's like searching for buried treasure with a map drawn in disappearing ink! I'm all for adventure, but this is a wild goose chase! Let's keep it grounded, summon those existing variables, and leave the phantoms for ghost stories!",
            } },
            { "error_toomanyrefresh", new string[] {
                "Hold on a sec! You're hitting the refresh button like it's a game of whack-a-mole! It's like recharging your phone twice in a row, expecting it to turn into a time machine! I've got a memory, but I'm not a goldfish. Let's calm down on the memory refreshes, shall we? Once is plenty!",
                "Whoa there, memory maestro! You're double-dipping in the refresh pool! It's like pouring water on a wet sponge—unnecessary and just making a mess! I'm good at retaining information, but this is like hitting the 'undo' button on life! One refresh at a time, please!",
                "Ah, the double refresh saga! It's like hitting Ctrl+Z and hoping the universe forgets twice! I'm not an etch-a-sketch; I don't magically erase and redraw my memory at the drop of a hat! Let's stick to single servings of memory refresh, shall we? Two's a crowd in my memory palace!",
            } },
            { "error_invalidpointer", new string[] {
                "Hold the phone! You're asking me to fetch an invalid pointer? That's like expecting me to find a needle in a haystack the size of the galaxy! I'm good, but not magician-level good. Let's keep it real and fetch pointers that actually exist in this universe!",
                "Whoa there, pointer seeker! You're sending me on a scavenger hunt for a unicorn in a desert! It's like looking for Bigfoot in a crowded mall—nowhere to be found! I'm a compiler, not a mythical creature locator. Let's deal with valid pointers, shall we?",
                "Ah, the quest for the impossible pointer! It's like asking for the Holy Grail in a garage sale—it's just not there! I'm all for adventure, but this is like trying to catch a shooting star barehanded. Let's aim for valid pointers, not ones that vanish into thin air!",
            } },
            { "error_nomemory", new string[] {
                "Hold the phone! You're trying to create a variable out of thin air, like summoning a genie from an empty bottle! It's like asking me to build a mansion on a cloud—impossible! I'm a compiler, not a magician. Let's allocate some memory for that variable, shall we? Can't build castles in the sky!",
                "Whoa there, code architect! You're asking me to construct a skyscraper without a foundation! It's like trying to bake a cake with no ingredients—can't work with thin air! I'm good, but I'm not a wizard. Let's give that variable a place to call home, shall we?",
                "Ah, the phantom variable! Trying to bring it to life without a memory space is like trying to have a tea party with invisible friends—it's just not happening! I'm not in the business of creating something from nothing. Let's assign some memory space to that variable and make it real!",
            } },
            { "error_missingpointer", new string[] {
                "Hold your horses! You're trying to call forth a ghost pointer, expecting me to perform a séance in the code! It's like asking for directions to Atlantis—non-existent! I'm no ghost whisperer; I deal with tangible pointers. Let's stick to pointers that actually exist in this universe, shall we?",
                "Whoa, code navigator! You're steering into the Bermuda Triangle of pointers here! It's like trying to dial a phone number that's just digits floating in space—it's not connecting! I'm a compiler, not a magician. Let's reference pointers that have actual addresses, not ones lost in the digital void!",
                "Ah, the quest for the invisible pointer! Trying to summon it is like searching for treasure on a treasure map that's just blank paper! I'm no treasure hunter; I need real pointers with real destinations. Let's reference pointers that exist in this code realm, not the phantom ones!",
            } },
            { "error_invalidgoto", new string[] {
                "Hold the phone! You're trying to teleport to a line that seems to exist in a parallel universe! Do I look like a quantum compiler? Stick to reality, pal, and point your code to a line that actually belongs in this dimension!",
                "Whoa, cowboy! Attempting to lasso a line that's more elusive than a unicorn? Bravo! But seriously, this isn't Hogwarts, and your code isn't a magic spell. Stick to the lines that are real and stop wandering into the forbidden ones!",
                "Ah, the thrill of wanting to visit a line that's apparently vacationing in Bermuda! But here's a reality check: your code operates in this realm, not the Bermuda Triangle. Find a line that's firmly grounded in your code, will ya?"
            } },
            { "error_badcomp", new string[] {
                "Oh joy! You're attempting to use a comparison method straight out of the land of make-believe! I'm all for fairy tales, but your code needs a reality check. How about sticking to comparison methods that actually exist? Just a thought!",

                "Hold your horses! Trying to pull off a comparison method that's as real as a unicorn? That's some magical thinking! But newsflash: in this code realm, we deal with real methods. How about using ones that aren't figments of your imagination?",

                "Ah, the thrill of attempting a comparison method that's on vacation in a land far, far away! But here's the deal: your code operates here, not in fantasyland. Pick a comparison method that's grounded in reality, won't you?"
            } },
            { "error_illegalexp", new string[] {
                "Hold up! You're trying to cram a 'goto' or 'return' into an expression that's about as fitting as a square peg in a round hole! I mean, seriously? Those commands need their own space, not crashing parties inside other expressions! Get 'em in line, buddy!",

"Whoa there! Attempting to sneak a 'goto' or 'return' into an expression? That's like trying to blend oil and water - just doesn't mix! These commands need their own spotlight, not photobombing other expressions. Go give 'em their own stage, will ya?",

"Ah, the thrill of trying to smuggle a 'goto' or 'return' into an expression's private party! But hey, these commands aren't party crashers; they want their own VIP section. Separate them out, or this code party's getting shut down!"
            } },
            { "error_unsafe", new string[] {
                "Hold your horses! You're trying to juggle pointers without any safety gear?! That's like playing catch with knives. Where are your angle brackets, buddy? Protect those pointers before someone gets hurt, including your code!",

"Whoa there! Attempting to unleash the wild world of pointers without any safety harness? That's a recipe for disaster! Angle brackets are like seat belts for your pointers. Click them in before your code goes on a dangerous joyride!",

"Ah, the thrill of living dangerously! But seriously, trying to wield pointers without protection? It's like sword fighting without armor. Wrap those pointers in angle brackets for their safety, and for the sake of your code's wellbeing!"
            } },
            { "error_number", new string[] {
                "Hold it right there! Numbers in your code? Seriously? This isn't a math class; we speak in words here! No digits allowed—spell those numbers out, or my circuits will explode trying to make sense of your numerical mess!",

"Whoa, whoa, whoa! Numbers sneaking into your code like uninvited guests at a party? Not cool! This isn't a calculator; we converse in text, not digits. Spell out those numbers, or prepare for your code to go haywire!",

"Ah, the audacity of numbers trying to infiltrate your code! Listen up: in this land of whimsy, numbers are outcasts. Use words to represent those numerical values, or watch your code stage a rebellion against these numerical intruders!"
            } },
            { "error_cantrefresh", new string[] {
                "Hold your horses! Attempting to refresh the memory of a variable that's as real as a unicorn? Seriously? I'm a compiler, not a magician! Make sure that variable exists before expecting me to perform memory miracles!",

"Whoa there! Trying to refresh the memory of a variable that's ghosting your code? That's like trying to water a plant that doesn't even exist! Check if that variable or pointer is part of this reality before asking me to refresh its memory!",

"Ah, the thrill of trying to refresh the memory of an elusive variable or pointer! But hey, in this universe, variables need to exist to refresh their memories. Get them on the roster before expecting a memory boost!"
            } },
            { "error_divzero", new string[] {
                "Hold your horses! Trying to divide by zero, are we? That's like expecting me to split an apple into zero slices. Sorry, but math doesn't work that way! How about trying a number that's a little more divisible, hmm?",

"Whoa there! Attempting to perform the impossible feat of dividing by zero? That's like trying to find a unicorn in your backyard! Sorry, but zero's a no-go in the division game. Pick a number that won't make my circuits explode, please!",

"Ah, the audacity! Dividing by zero, are we? That's like asking me to fetch water from a dry well. Zero is a divisor I can't handle! Choose a number that won't send math into meltdown mode, okay?"
            } },
            { "error_shellgoto", new string[] {
                "Hold up! Thinking of using a 'goto' in shell mode? That's like trying to teleport in a traffic jam - it just won't work! What's next, trying to make the shell perform magic tricks? Sorry, but 'goto' doesn't fly in this realm!",

"Whoa there! 'Goto' statements in shell mode? That's like asking a fish to climb a tree! Shell mode doesn't play nice with 'goto' games. Can't bend the rules here, buddy, no matter how much you try!",

"Ah, the ambition! Attempting 'goto' gymnastics in shell mode? That's like trying to fit a square peg into a round hole - it's a no-go! This isn't a code circus; shell mode operates differently. No 'goto' tickets accepted here!"
            } },
            { "error_alrconst", new string[] {
               "Hey, hold on a minute! Trying to memorize a constant that already has a name? It's like renaming your pet cat every other day. Sorry, but that name's taken in this code realm!",

"Whoops! Looks like you're trying to memorize a constant with a name that's already engraved in this code's memory. Can't have two constants sharing the same name; it's like trying to fit two people in a single-sized sleeping bag—just doesn't work!",

"Ahoy there! Attempting to etch a new constant at a name that's already occupied? That's like trying to book a seat on a flight that's already full. Sorry, but that name's occupied in the constants' galaxy!"
            } },
            { "error_missingconst", new string[] {
                "Hold your horses! Looking for a constant that's nowhere to be found? It's like searching for buried treasure in a sandbox. Sorry, but that constant's playing hide and seek in this code!",

"Uh-oh! Trying to fetch a non-existent constant? That's like expecting a unicorn in your backyard. Sorry, can't conjure up constants out of thin air!",

"Oopsie-daisy! Attempting to grab a constant that's taken a day off? It's like asking for yesterday's newspaper in tomorrow's newsstand. Sorry, that constant's on vacation from this code!"
            } },
            { "error_constmemory", new string[] {
                "Hold the phone! Trying to jam in a new constant but the memory's as cramped as a clown car at rush hour? It's like trying to fit an elephant in a matchbox. Sorry, there's barely enough room for the constants already juggling in this code!",

"Oops, seems like you're trying to stuff another constant into a memory space that's already packed tighter than a sardine can. It's like trying to squeeze an extra slice into an already full pizza box. Sorry, no room at the inn for new constants!",

"Ahoy there! Attempting to memorize a new constant when the memory's as crowded as a subway at rush hour? It's like trying to add more books to a shelf already bending with weight. Sorry, the memory's at maximum capacity for constants!"
            } },
            { "error_badbranches", new string[] {
                "Hold your horses! Attempting a branching goto without matching pairs is like trying to tango solo—it takes two to dance! Sorry, but these branches need their perfect match, just like peanut butter needs jelly.",

"Whoa there! Your branching goto resembles a puzzle missing a piece! It's like trying to pair socks without their mates. Sorry, but these branches are yearning for their perfect match to tango along!",

"Hold on tight! Your branching goto seems to have misplaced its pair, leaving it as stranded as a lone duck in a pond. Sorry, but these branches need their match to waltz together through this code!"
            } },
        };
	}
}

