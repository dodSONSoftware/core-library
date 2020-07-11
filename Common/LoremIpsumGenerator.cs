using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Common
{
    /// <summary>
    /// Generates randomized text from either the default, embedded, Lorem Ipsum resource or a custom Lorem Ipsum resource.
    /// </summary>
    /// <example>
    /// The following code example will create a lorem ipsum resource from example text then use that resource to generate 5 paragraphs; 
    /// then it will generate 5 paragraphs from the default, embedded, lorem ipsum resource.
    /// <para>
    /// Create a console application and add the following code:
    /// </para>
    /// <code>
    /// static void Main(string[] args)
    /// {
    ///     // display the first default lorem ipsum sentence
    ///     Console.WriteLine(dodSON.Core.Common.LoremIpsumGenerator.FirstSentence);
    ///     Console.WriteLine();
    ///     Console.WriteLine("------------------------");
    ///     Console.WriteLine();
    /// 
    ///     // create example text to be analyzed
    ///     // each line represents a paragraph. In other words, the Environment.Newline separates the paragraphs.
    ///     // 
    ///     var firstSentence = "The Hound of the Baskervilles is the third of the crime novels written by Sir Arthur Conan Doyle featuring the detective Sherlock Holmes. Originally serialized in The Strand Magazine from August 1901 to April 1902, it is set largely on Dartmoor in Devon in England's West Country and tells the story of an attempted murder inspired by the legend of a fearsome, diabolical hound of supernatural origin.";
    ///     var exampleText = "I have in my pocket a manuscript, said Dr. James Mortimer." + Environment.NewLine +
    ///                       "I observed it as you entered the room, said Holmes." + Environment.NewLine +
    ///                       "It is an old manuscript." + Environment.NewLine +
    ///                       "Early eighteenth century, unless it is a forgery." + Environment.NewLine +
    ///                       "How can you say that, sir?" + Environment.NewLine +
    ///                       "You have presented an inch or two of it to my examination all the time that you have been talking. It would be a poor expert who could not give the date of a document within a decade or so. You may possibly have read my little monograph upon the subject. I put that at 1730." + Environment.NewLine +
    ///                       "The exact date is 1742. Dr. Mortimer drew it from his breast-pocket. This family paper was committed to my care by Sir Charles Baskerville, whose sudden and tragic death some three months ago created so much excitement in Devonshire. I may say that I was his personal friend as well as his medical attendant. He was a strong-minded man, sir, shrewd, practical, and as unimaginative as I am myself. Yet he took this document very seriously, and his mind was prepared for just such an end as did eventually overtake him." + Environment.NewLine +
    ///                       "Holmes stretched out his hand for the manuscript and flattened it upon his knee." + Environment.NewLine +
    ///                       "You will observe, Watson, the alternative use of the long s and the short. It is one of several indications which enabled me to fix the date." + Environment.NewLine +
    ///                       "I looked over his shoulder at the yellow paper and the faded script. At the head was written: Baskerville Hall, and below in large, scrawling figures: 1742." + Environment.NewLine +
    ///                       "It appears to be a statement of some sort." + Environment.NewLine +
    ///                       "Yes, it is a statement of a certain legend which runs in the Baskerville family." + Environment.NewLine +
    ///                       "But I understand that it is something more modern and practical upon which you wish to consult me?" + Environment.NewLine +
    ///                       "Most modern. A most practical, pressing matter, which must be decided within twenty-four hours. But the manuscript is short and is intimately connected with the affair. With your permission I will read it to you." + Environment.NewLine +
    ///                       "Holmes leaned back in his chair, placed his finger-tips together, and closed his eyes, with an air of resignation. Dr. Mortimer turned the manuscript to the light and read in a high, cracking voice the following curious, old-world narrative:—" + Environment.NewLine +
    ///                       "Of the origin of the Hound of the Baskervilles there have been many statements, yet as I come in a direct line from Hugo Baskerville, and as I had the story from my father, who also had it from his, I have set it down with all belief that it occurred even as is here set forth. And I would have you believe, my sons, that the same Justice which punishes sin may also most graciously forgive it, and that no ban is so heavy but that by prayer and repentance it may be removed. Learn then from this story not to fear the fruits of the past, but rather to be circumspect in the future, that those foul passions whereby our family has suffered so grievously may not again be loosed to our undoing." + Environment.NewLine +
    ///                       "Know then that in the time of the Great Rebellion (the history of which by the learned Lord Clarendon I most earnestly commend to your attention) this Manor of Baskerville was held by Hugo of that name, nor can it be gainsaid that he was a most wild, profane, and godless man. This, in truth, his neighbours might have pardoned, seeing that saints have never flourished in those parts, but there was in him a certain wanton and cruel humour which made his name a byword through the West. It chanced that this Hugo came to love (if, indeed, so dark a passion may be known under so bright a name) the daughter of a yeoman who held lands near the Baskerville estate. But the young maiden, being discreet and of good repute, would ever avoid him, for she feared his evil name. So it came to pass that one Michaelmas this Hugo, with five or six of his idle and wicked companions, stole down upon the farm and carried off the maiden, her father and brothers being from home, as he well knew. When they had brought her to the Hall the maiden was placed in an upper chamber, while Hugo and his friends sat down to a long carouse, as was their nightly custom. Now, the poor lass upstairs was like to have her wits turned at the singing and shouting and terrible oaths which came up to her from below, for they say that the words used by Hugo Baskerville, when he was in wine, were such as might blast the man who said them. At last in the stress of her fear she did that which might have daunted the bravest or most active man, for by the aid of the growth of ivy which covered (and still covers) the south wall she came down from under the eaves, and so homeward across the moor, there being three leagues betwixt the Hall and her father's farm." + Environment.NewLine +
    ///                       "It chanced that some little time later Hugo left his guests to carry food and drink—with other worse things, perchance—to his captive, and so found the cage empty and the bird escaped. Then, as it would seem, he became as one that hath a devil, for, rushing down the stairs into the dining-hall, he sprang upon the great table, flagons and trenchers flying before him, and he cried aloud before all the company that he would that very night render his body and soul to the Powers of Evil if he might but overtake the wench. And while the revellers stood aghast at the fury of the man, one more wicked or, it may be, more drunken than the rest, cried out that they should put the hounds upon her. Whereat Hugo ran from the house, crying to his grooms that they should saddle his mare and unkennel the pack, and giving the hounds a kerchief of the maid's, he swung them to the line, and so off full cry in the moonlight over the moor." + Environment.NewLine +
    ///                       "Now, for some space the revellers stood agape, unable to understand all that had been done in such haste. But anon their bemused wits awoke to the nature of the deed which was like to be done upon the moorlands. Everything was now in an uproar, some calling for their pistols, some for their horses, and some for another flask of wine. But at length some sense came back to their crazed minds, and the whole of them, thirteen in number, took horse and started in pursuit. The moon shone clear above them, and they rode swiftly abreast, taking that course which the maid must needs have taken if she were to reach her own home." + Environment.NewLine +
    ///                       "They had gone a mile or two when they passed one of the night shepherds upon the moorlands, and they cried to him to know if he had seen the hunt. And the man, as the story goes, was so crazed with fear that he could scarce speak, but at last he said that he had indeed seen the unhappy maiden, with the hounds upon her track. 'But I have seen more than that,' said he, 'for Hugo Baskerville passed me upon his black mare, and there ran mute behind him such a hound of hell as God forbid should ever be at my heels.' So the drunken squires cursed the shepherd and rode onward. But soon their skins turned cold, for there came a galloping across the moor, and the black mare, dabbled with white froth, went past with trailing bridle and empty saddle. Then the revellers rode close together, for a great fear was on them, but they still followed over the moor, though each, had he been alone, would have been right glad to have turned his horse's head. Riding slowly in this fashion they came at last upon the hounds. These, though known for their valour and their breed, were whimpering in a cluster at the head of a deep dip or goyal, as we call it, upon the moor, some slinking away and some, with starting hackles and staring eyes, gazing down the narrow valley before them." + Environment.NewLine +
    ///                       "The company had come to a halt, more sober men, as you may guess, than when they started. The most of them would by no means advance, but three of them, the boldest, or it may be the most drunken, rode forward down the goyal. Now, it opened into a broad space in which stood two of those great stones, still to be seen there, which were set by certain forgotten peoples in the days of old. The moon was shining bright upon the clearing, and there in the centre lay the unhappy maid where she had fallen, dead of fear and of fatigue. But it was not the sight of her body, nor yet was it that of the body of Hugo Baskerville lying near her, which raised the hair upon the heads of these three daredevil roysterers, but it was that, standing over Hugo, and plucking at his throat, there stood a foul thing, a great, black beast, shaped like a hound, yet larger than any hound that ever mortal eye has rested upon. And even as they looked the thing tore the throat out of Hugo Baskerville, on which, as it turned its blazing eyes and dripping jaws upon them, the three shrieked with fear and rode for dear life, still screaming, across the moor. One, it is said, died that very night of what he had seen, and the other twain were but broken men for the rest of their days." + Environment.NewLine +
    ///                       "Such is the tale, my sons, of the coming of the hound which is said to have plagued the family so sorely ever since. If I have set it down it is because that which is clearly known hath less terror than that which is but hinted at and guessed. Nor can it be denied that many of the family have been unhappy in their deaths, which have been sudden, bloody, and mysterious. Yet may we shelter ourselves in the infinite goodness of Providence, which would not forever punish the innocent beyond that third or fourth generation which is threatened in Holy Writ. To that Providence, my sons, I hereby commend you, and I counsel you by way of caution to forbear from crossing the moor in those dark hours when the powers of evil are exalted." + Environment.NewLine +
    ///                       "[This from Hugo Baskerville to his sons Rodger and John, with instructions that they say nothing thereof to their sister Elizabeth.]" + Environment.NewLine +
    ///                       "When Dr. Mortimer had finished reading this singular narrative he pushed his spectacles up on his forehead and stared across at Mr. Sherlock Holmes. The latter yawned and tossed the end of his cigarette into the fire." + Environment.NewLine +
    ///                       "Well? said he." + Environment.NewLine +
    ///                       "Do you not find it interesting?" + Environment.NewLine +
    ///                       "To a collector of fairy tales." + Environment.NewLine +
    ///                       "Dr. Mortimer drew a folded newspaper out of his pocket." + Environment.NewLine +
    ///                       "Now, Mr. Holmes, we will give you something a little more recent. This is the Devon County Chronicle of May 14th of this year. It is a short account of the facts elicited at the death of Sir Charles Baskerville which occurred a few days before that date." + Environment.NewLine +
    ///                       "My friend leaned a little forward and his expression became intent. Our visitor readjusted his glasses and began:—" + Environment.NewLine +
    ///                       "The recent sudden death of Sir Charles Baskerville, whose name has been mentioned as the probable Liberal candidate for Mid-Devon at the next election, has cast a gloom over the county. Though Sir Charles had resided at Baskerville Hall for a comparatively short period his amiability of character and extreme generosity had won the affection and respect of all who had been brought into contact with him. In these days of nouveaux riches it is refreshing to find a case where the scion of an old county family which has fallen upon evil days is able to make his own fortune and to bring it back with him to restore the fallen grandeur of his line. Sir Charles, as is well known, made large sums of money in South African speculation. More wise than those who go on until the wheel turns against them, he realized his gains and returned to England with them. It is only two years since he took up his residence at Baskerville Hall, and it is common talk how large were those schemes of reconstruction and improvement which have been interrupted by his death. Being himself childless, it was his openly expressed desire that the whole country-side should, within his own lifetime, profit by his good fortune, and many will have personal reasons for bewailing his untimely end. His generous donations to local and county charities have been frequently chronicled in these columns." + Environment.NewLine +
    ///                       "The circumstances connected with the death of Sir Charles cannot be said to have been entirely cleared up by the inquest, but at least enough has been done to dispose of those rumours to which local superstition has given rise. There is no reason whatever to suspect foul play, or to imagine that death could be from any but natural causes. Sir Charles was a widower, and a man who may be said to have been in some ways of an eccentric habit of mind. In spite of his considerable wealth he was simple in his personal tastes, and his indoor servants at Baskerville Hall consisted of a married couple named Barrymore, the husband acting as butler and the wife as housekeeper. Their evidence, corroborated by that of several friends, tends to show that Sir Charles's health has for some time been impaired, and points especially to some affection of the heart, manifesting itself in changes of colour, breathlessness, and acute attacks of nervous depression. Dr. James Mortimer, the friend and medical attendant of the deceased, has given evidence to the same effect." + Environment.NewLine +
    ///                       "The facts of the case are simple. Sir Charles Baskerville was in the habit every night before going to bed of walking down the famous Yew Alley of Baskerville Hall. The evidence of the Barrymores shows that this had been his custom. On the 4th of May Sir Charles had declared his intention of starting next day for London, and had ordered Barrymore to prepare his luggage. That night he went out as usual for his nocturnal walk, in the course of which he was in the habit of smoking a cigar. He never returned. At twelve o'clock Barrymore, finding the hall door still open, became alarmed, and, lighting a lantern, went in search of his master. The day had been wet, and Sir Charles's footmarks were easily traced down the Alley. Half-way down this walk there is a gate which leads out on to the moor. There were indications that Sir Charles had stood for some little time here. He then proceeded down the Alley, and it was at the far end of it that his body was discovered. One fact which has not been explained is the statement of Barrymore that his master's footprints altered their character from the time that he passed the moor-gate, and that he appeared from thence onward to have been walking upon his toes. One Murphy, a gipsy horse-dealer, was on the moor at no great distance at the time, but he appears by his own confession to have been the worse for drink. He declares that he heard cries, but is unable to state from what direction they came. No signs of violence were to be discovered upon Sir Charles's person, and though the doctor's evidence pointed to an almost incredible facial distortion—so great that Dr. Mortimer refused at first to believe that it was indeed his friend and patient who lay before him—it was explained that that is a symptom which is not unusual in cases of dyspnoea and death from cardiac exhaustion. This explanation was borne out by the post-mortem examination, which showed long-standing organic disease, and the coroner's jury returned a verdict in accordance with the medical evidence. It is well that this is so, for it is obviously of the utmost importance that Sir Charles's heir should settle at the Hall and continue the good work which has been so sadly interrupted. Had the prosaic finding of the coroner not finally put an end to the romantic stories which have been whispered in connection with the affair, it might have been difficult to find a tenant for Baskerville Hall. It is understood that the next of kin is Mr. Henry Baskerville, if he be still alive, the son of Sir Charles Baskerville's younger brother. The young man when last heard of was in America, and inquiries are being instituted with a view to informing him of his good fortune." + Environment.NewLine +
    ///                       "Dr. Mortimer refolded his paper and replaced it in his pocket." + Environment.NewLine +
    ///                       "Those are the public facts, Mr. Holmes, in connection with the death of Sir Charles Baskerville." + Environment.NewLine +
    ///                       "I must thank you, said Sherlock Holmes, for calling my attention to a case which certainly presents some features of interest. I had observed some newspaper comment at the time, but I was exceedingly preoccupied by that little affair of the Vatican cameos, and in my anxiety to oblige the Pope I lost touch with several interesting English cases. This article, you say, contains all the public facts?" + Environment.NewLine +
    ///                       "It does." + Environment.NewLine +
    ///                       "Then let me have the private ones. He leaned back, put his finger-tips together, and assumed his most impassive and judicial expression." + Environment.NewLine +
    ///                       "In doing so, said Dr. Mortimer, who had begun to show signs of some strong emotion, I am telling that which I have not confided to anyone. My motive for withholding it from the coroner's inquiry is that a man of science shrinks from placing himself in the public position of seeming to indorse a popular superstition. I had the further motive that Baskerville Hall, as the paper says, would certainly remain untenanted if anything were done to increase its already rather grim reputation. For both these reasons I thought that I was justified in telling rather less than I knew, since no practical good could result from it, but with you there is no reason why I should not be perfectly frank." + Environment.NewLine +
    ///                       "The moor is very sparsely inhabited, and those who live near each other are thrown very much together. For this reason I saw a good deal of Sir Charles Baskerville. With the exception of Mr. Frankland, of Lafter Hall, and Mr. Stapleton, the naturalist, there are no other men of education within many miles. Sir Charles was a retiring man, but the chance of his illness brought us together, and a community of interests in science kept us so. He had brought back much scientific information from South Africa, and many a charming evening we have spent together discussing the comparative anatomy of the Bushman and the Hottentot." + Environment.NewLine +
    ///                       "Within the last few months it became increasingly plain to me that Sir Charles's nervous system was strained to the breaking point. He had taken this legend which I have read you exceedingly to heart—so much so that, although he would walk in his own grounds, nothing would induce him to go out upon the moor at night. Incredible as it may appear to you, Mr. Holmes, he was honestly convinced that a dreadful fate overhung his family, and certainly the records which he was able to give of his ancestors were not encouraging. The idea of some ghastly presence constantly haunted him, and on more than one occasion he has asked me whether I had on my medical journeys at night ever seen any strange creature or heard the baying of a hound. The latter question he put to me several times, and always with a voice which vibrated with excitement." + Environment.NewLine +
    ///                       "I can well remember driving up to his house in the evening some three weeks before the fatal event. He chanced to be at his hall door. I had descended from my gig and was standing in front of him, when I saw his eyes fix themselves over my shoulder, and stare past me with an expression of the most dreadful horror. I whisked round and had just time to catch a glimpse of something which I took to be a large black calf passing at the head of the drive. So excited and alarmed was he that I was compelled to go down to the spot where the animal had been and look around for it. It was gone, however, and the incident appeared to make the worst impression upon his mind. I stayed with him all the evening, and it was on that occasion, to explain the emotion which he had shown, that he confided to my keeping that narrative which I read to you when first I came. I mention this small episode because it assumes some importance in view of the tragedy which followed, but I was convinced at the time that the matter was entirely trivial and that his excitement had no justification." + Environment.NewLine +
    ///                       "It was at my advice that Sir Charles was about to go to London. His heart was, I knew, affected, and the constant anxiety in which he lived, however chimerical the cause of it might be, was evidently having a serious effect upon his health. I thought that a few months among the distractions of town would send him back a new man. Mr. Stapleton, a mutual friend who was much concerned at his state of health, was of the same opinion. At the last instant came this terrible catastrophe." + Environment.NewLine +
    ///                       "On the night of Sir Charles's death Barrymore the butler, who made the discovery, sent Perkins the groom on horseback to me, and as I was sitting up late I was able to reach Baskerville Hall within an hour of the event. I checked and corroborated all the facts which were mentioned at the inquest. I followed the footsteps down the Yew Alley, I saw the spot at the moor-gate where he seemed to have waited, I remarked the change in the shape of the prints after that point, I noted that there were no other footsteps save those of Barrymore on the soft gravel, and finally I carefully examined the body, which had not been touched until my arrival. Sir Charles lay on his face, his arms out, his fingers dug into the ground, and his features convulsed with some strong emotion to such an extent that I could hardly have sworn to his identity. There was certainly no physical injury of any kind. But one false statement was made by Barrymore at the inquest. He said that there were no traces upon the ground round the body. He did not observe any. But I did—some little distance off, but fresh and clear." + Environment.NewLine +
    ///                       "Footprints?" + Environment.NewLine +
    ///                       "Footprints." + Environment.NewLine +
    ///                       "A man's or a woman's?" + Environment.NewLine +
    ///                       "Dr. Mortimer looked strangely at us for an instant, and his voice sank almost to a whisper as he answered:—" + Environment.NewLine +
    ///                       "Mr. Holmes, they were the footprints of a gigantic hound!";
    /// 
    ///     // analyze the example text for paragraph and sentence structure and extract all words
    ///     var resource = dodSON.Core.Common.LoremIpsumGenerator.CreateLoremIpsumConfiguration(firstSentence, exampleText);
    /// 
    ///     // generate random text based on the analyzed string 
    ///     Console.WriteLine(dodSON.Core.Common.LoremIpsumGenerator.GenerateChapter(resource, 5, true));
    ///     Console.WriteLine();
    ///     Console.WriteLine("------------------------");
    ///     Console.WriteLine();
    /// 
    ///     // generate random text based on the default, embedded, lorem ipsum resource
    ///     Console.WriteLine(dodSON.Core.Common.LoremIpsumGenerator.GenerateChapter(5, true));
    ///     Console.WriteLine();
    ///     Console.WriteLine("------------------------");
    ///     Console.WriteLine("press anykey...");
    ///     Console.ReadKey(true);
    /// }
    /// 
    /// // This code produces output similar to the following:
    /// 
    /// // Lorem ipsum dolor sit amet, consectetur adipiscing elit.
    /// // 
    /// // ------------------------
    /// // 
    /// // The Hound of the Baskervilles is the third of the crime novels written by Sir Arthur Conan Doyle featuring the 
    /// // detective Sherlock Holmes. Originally serialized in The Strand Magazine from August 1901 to April 1902, it is 
    /// // set largely on Dartmoor in Devon in England's West Country and tells the story of an attempted murder inspired 
    /// // by the legend of a fearsome, diabolical hound of supernatural origin. In is statement time signs stretched 
    /// // their his or say. Their the as her of his understood that to hounds. A was tale may of the his the an opinion.
    /// // 
    /// // Fatigue bushman reach luggage were still i manifesting their his believe the been this there one so hugo would 
    /// // to it. His to be but kerchief then south that the hall before and of still keeping james intention henry perfectly 
    /// // to when. To such it as seeming the should turned the that his been you turned talking that hall heart was of 
    /// // withholding. Is stood into the by a who personal it the round and to eyes mortimer dininghall a replaced saddle at heels'.
    /// // 
    /// // Account unless months later sir upon be which result i the. Of easily as lost fear to each but the connection but. 
    /// // He has must companions the henry it some large days of. Into servants sorely entirely fruits the has bloody the at 
    /// // resided. Raised an the interrupted connected been as little to i advance. South hath forth good said for have of to 
    /// // the of? Baying death story the some his a toes being alternative from.
    /// // 
    /// // Covers you upon search that no be direction an and his. That hugo being as let across in then love had evening. 
    /// // Attention stones any the upon may shrieked the is was those. Been of brought had character which out spite forgive 
    /// // and was. Voice that charles's of family to injury rode three heart and. In her dr and have great the through 
    /// // would had consult. Infinite man attention view his of few has have for state.
    /// // 
    /// // Their interest an sir it incident home examination when i as maid's that many been to time suffered him in of they. 
    /// // His hounds he may held down in character saddle out attention a begun pushed in my of the date my information crying. 
    /// // Have younger upon his wall had upon soon him with all of consult driving that would but men of come attendant this? 
    /// // Occurred thought he poor death white and fairy family punishes shoulder been was to the to of statements mare master 
    /// // were we. Same that followed parts of well for is had last and punish the to fallen charles hall since more had for since.
    /// // 
    /// // ------------------------
    /// // 
    /// // Lorem ipsum dolor sit amet, consectetur adipiscing elit. Etiam amet mollis in nibh pharetra a. Dictum lorem nec id 
    /// // phasellus egestas venenatis. Vitae mattis malesuada rhoncus morbi diam et. Sit elementum in suscipit phasellus consectetur 
    /// // in. Porttitor ipsum id et lacus pellentesque arcu. In mattis auctor efficitur vestibulum vitae nullam.
    /// // 
    /// // At sed ut sapien egestas porta. Praesent euismod aenean mi id pretium. Vulputate neque massa tincidunt vestibulum 
    /// // luctus. Sed purus viverra consequat fames ultricies. Curabitur ac neque vel egestas tristique. Sed tincidunt vestibulum 
    /// // posuere vulputate rhoncus. Ut dui imperdiet quam id hendrerit. Feugiat velit mauris at lectus pharetra. Finibus ex et 
    /// // consequat id sed.
    /// // 
    /// // Risus etiam aliquet in fringilla lobortis. Vitae ante aliquet sed lobortis sodales. Nunc efficitur ullamcorper augue 
    /// // eu eros. Ullamcorper interdum ullamcorper pellentesque consequat proin. Rutrum magna dis amet mauris massa. Dui neque 
    /// // aliquet phasellus at volutpat. Risus ultricies ipsum vitae vel praesent. Lacinia mauris pellentesque varius volutpat 
    /// // consequat. Velit lectus consectetur condimentum nam aenean. Gravida pharetra velit sed nisl ut. Consequat nibh lacus 
    /// // a eros augue. Consequat pretium sed eros pulvinar bibendum.
    /// // 
    /// // Quis pretium nibh tristique ligula porttitor eu vitae dapibus finibus. Id aliquam consectetur ultricies ac ut nunc 
    /// // ut ullamcorper viverra. Dolor maecenas magnis duis cras tellus odio euismod id in. Et mus risus hendrerit non mollis 
    /// // tincidunt maecenas fusce himenaeos. Nibh sed orci nulla ultricies eu nulla fringilla sollicitudin dis. Volutpat mollis 
    /// // quam amet dapibus aliquet convallis mauris interdum aliquam. Primis eros nibh nam sed tortor ornare integer donec sed.
    /// // 
    /// // Nec phasellus viverra nec porta pretium quisque nunc aenean ligula ac. Quis nam sed egestas malesuada facilisis ut 
    /// // nibh condimentum at aliquam. Orci viverra ac id diam nisi egestas turpis odio in velit. Vel massa varius mauris quam 
    /// // fringilla bibendum maximus in interdum curabitur. Nisi purus facilisis eros nisl sagittis metus libero condimentum 
    /// // mauris mollis. Nulla consectetur nunc orci proin justo ac libero consequat orci tempor. Morbi tincidunt augue iaculis 
    /// // id ex arcu dictum luctus quis vestibulum. In gravida vivamus porta sed suscipit egestas suspendisse nisi mi erat. 
    /// // Mollis neque nec neque mauris purus nibh id massa dolor rhoncus. Mattis felis ipsum ligula integer hendrerit egestas 
    /// // finibus justo amet nam. Accumsan eget libero felis erat sem penatibus quam elit dignissim nam. Bibendum amet aliquam 
    /// // ante amet nulla pretium aenean sed urna ut. Adipiscing ligula sed mus vestibulum diam malesuada tortor urna auctor semper.
    /// // 
    /// // ------------------------
    /// // press anykey...
    /// </code>
    /// </example>    
    public static class LoremIpsumGenerator
    {
        #region Private Fields
        private static readonly string _EmbeddedResourceFilename = "_Resources.LoremIpsum.resource";
        private static readonly int _SentenceMinimumWordCount = 6;
        private static readonly int _SentenceMaximumWordCount = 24;
        private static readonly int _ParagraphMinimumSentenceCount = 4;
        private static readonly int _ParagraphMaximumSentenceCount = 32;
        private static readonly string _ValidPunctuationMarks = ".?!";
        private static readonly string _ValidPunctuationUsedAsCharsMarks = "'";
        private static readonly int _StringBuilderChapterDefaultCapacity_ = 102400;
        private static readonly int _StringBuilderParagraphDefaultCapacity_ = 10240;
        private static readonly int _StringBuilderSentenceDefaultCapacity_ = 1024;
        private static readonly int _StringBuilderWordDefaultCapacity_ = 32;
        //
        private static WeakReference _UncompressedEmbeddedResource = null;
        private static string _LastWordGenerated = "";
        private static Random _Random = null;
        #endregion
        #region Public Methods
        // ******** Create Methods
        /// <summary>
        /// Will analyze the given text, gathering statistics on paragraph and sentence structure, punctuation patterns and record all the words and their display frequency.
        /// </summary>
        /// <param name="sourceTextToAnalyze">The text to analyze.</param>
        /// <param name="firstSentence">Represents the first sentence for the returned lorem ipsum resource.</param>
        /// <returns>A <see cref="LoremIpsumConfiguration"/> class containing the necessary information for the text generators to create random, simulated text based on the given text.</returns>
        public static LoremIpsumConfiguration CreateLoremIpsumConfiguration(string firstSentence,
                                                                            string sourceTextToAnalyze)
        {
            if (string.IsNullOrWhiteSpace(firstSentence))
            {
                throw new ArgumentNullException(nameof(firstSentence));
            }
            if (string.IsNullOrWhiteSpace(sourceTextToAnalyze))
            {
                throw new ArgumentNullException(nameof(sourceTextToAnalyze));
            }
            // analyze text file
            var sentencePatterns = new Dictionary<int, int>();
            var paragraphPatterns = new Dictionary<int, int>();
            var punctuationPatterns = new Dictionary<char, int>();
            var words = new Dictionary<int, List<LoremIpsumTextAnalysisData>>();
            var workerWord = new StringBuilder(_StringBuilderWordDefaultCapacity_);
            var whiteSpaceCounter = 0;
            var wordCount = 0;
            var sentenceCount = 0;
            var textToAnalyze = sourceTextToAnalyze.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (var item in textToAnalyze)
            {
                var line = item.Trim() + ' ';
                if (line.Length == 0)
                {
                    ++whiteSpaceCounter;
                }
                else
                {
                    foreach (var ch in line)
                    {
                        if (ch == ' ')
                        {
                            ++whiteSpaceCounter;
                            // end of word
                            if (workerWord.Length > 0)
                            {
                                // process word
                                ++wordCount;
                                var candidateWord = workerWord.ToString().Trim().ToLower();
                                workerWord.Clear();
                                if (!string.IsNullOrWhiteSpace(candidateWord))
                                {
                                    // sort by word length
                                    var wordLength = candidateWord.Length;
                                    if (!words.ContainsKey(wordLength))
                                    {
                                        // add to dictionary
                                        words.Add(wordLength, new List<LoremIpsumTextAnalysisData>());
                                        words[wordLength].Add(new LoremIpsumTextAnalysisData() { Word = candidateWord, Count = 1 });
                                    }
                                    else
                                    {
                                        // find word dataholder
                                        var data = words[wordLength].Find(new Predicate<LoremIpsumTextAnalysisData>((dh) => { return dh.Word == candidateWord; }));
                                        if (data != null)
                                        {
                                            // update word frequency counter
                                            data.Count++;
                                        }
                                        else
                                        {
                                            // add word
                                            words[wordLength].Add(new LoremIpsumTextAnalysisData() { Word = candidateWord, Count = 1 });
                                        }
                                    }
                                }
                            }
                        }
                        else if (_ValidPunctuationMarks.Contains(ch))
                        {
                            // gather punctuation statistics
                            if (punctuationPatterns.ContainsKey(ch))
                            {
                                ++punctuationPatterns[ch];
                            }
                            else
                            {
                                punctuationPatterns.Add(ch, 1);
                            }

                            // end of sentence
                            ++sentenceCount;
                            if ((wordCount >= _SentenceMinimumWordCount) && (wordCount <= _SentenceMaximumWordCount))
                            {
                                if (sentencePatterns.ContainsKey(wordCount))
                                {
                                    ++sentencePatterns[wordCount];
                                }
                                else
                                {
                                    sentencePatterns.Add(wordCount, 1);
                                }
                            }
                            wordCount = 0;
                        }
                        else if (char.IsLetterOrDigit(ch) ||
                                 _ValidPunctuationUsedAsCharsMarks.Contains(ch))
                        {
                            // build word
                            workerWord.Append(ch);
                        }
                    }
                    // end of paragraph
                    if ((sentenceCount >= _ParagraphMinimumSentenceCount) && (sentenceCount <= _ParagraphMaximumSentenceCount))
                    {
                        if (paragraphPatterns.ContainsKey(sentenceCount))
                        {
                            ++paragraphPatterns[sentenceCount];
                        }
                        else
                        {
                            paragraphPatterns.Add(sentenceCount, 1);
                        }
                    }
                    sentenceCount = 0;
                }
            }
            // prepare data to save to resource file
            var allWordsConverted = new Dictionary<string, int>();
            foreach (var item in words.SelectMany(x => { return x.Value; }))
            {
                allWordsConverted.Add(item.Word, item.Count);
            }
            var sentencePatternsOrdered = new Dictionary<int, int>();
            foreach (var item in from x in sentencePatterns
                                 orderby x.Key ascending
                                 select x)
            {
                sentencePatternsOrdered.Add(item.Key, item.Value);
            }
            var paragraphPatternsOrdered = new Dictionary<int, int>();
            foreach (var item in from x in paragraphPatterns
                                 orderby x.Key ascending
                                 select x)
            {
                paragraphPatternsOrdered.Add(item.Key, item.Value);
            }
            // create lorem ipsum configuration
            var loremIpsumConfig = new LoremIpsumConfiguration(firstSentence,
                                                               sentencePatternsOrdered,
                                                               paragraphPatternsOrdered,
                                                               punctuationPatterns,
                                                               allWordsConverted);
            //
            return loremIpsumConfig;
        }
        /// <summary>
        /// Will analyze the text in the given file, gathering statistics on paragraph and sentence structure, punctuation patterns and record all the words and their display frequency.
        /// The first sentence of the first paragraph will be used as the first sentence for the <see cref="LoremIpsumConfiguration"/> class's <see cref="LoremIpsumConfiguration.FirstSentence"/> property.
        /// </summary>
        /// <param name="filename">The text file to analyze.</param>
        /// <returns>A <see cref="LoremIpsumConfiguration"/> class containing the necessary information for the text generators to create random, simulated text based on the text in the specified file.</returns>
        public static LoremIpsumConfiguration CreateLoremIpsumConfiguration(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentNullException(nameof(filename));
            }
            if (!System.IO.File.Exists(filename))
            {
                throw new System.IO.FileNotFoundException(filename);
            }
            var textToAnalyze = "";
            using (var sr = new System.IO.StreamReader(filename))
            {
                // get text to analyze
                textToAnalyze = sr.ReadToEnd().Trim();
                sr.Close();
            }
            // extract the first sentence
            var validPunctuationMarksIndex = -1;
            for (int i = 0; i < textToAnalyze.Length; i++)
            {
                if (_ValidPunctuationMarks.Contains(textToAnalyze[i]))
                {
                    validPunctuationMarksIndex = i;
                    break;
                }
            }
            if (validPunctuationMarksIndex == -1)
            {
                validPunctuationMarksIndex = textToAnalyze.Length - 1;
            }
            var firstSentence = textToAnalyze.Substring(0, validPunctuationMarksIndex + 1).Trim();
            if (String.IsNullOrWhiteSpace(firstSentence))
            {
                // try finding anything that is not a digit or a character or a blank
                var baseSearchIndex = 0;
                for (int i = 0; i < textToAnalyze.Length; i++)
                {
                    if (!((char.IsLetterOrDigit(textToAnalyze[i])) || (textToAnalyze[i] != ' ')))
                    {
                        baseSearchIndex = i;
                        break;
                    }
                }
                firstSentence = textToAnalyze.Substring(0, baseSearchIndex);
            }
            return CreateLoremIpsumConfiguration(firstSentence, textToAnalyze);
        }
        // ******** Conversion Methods
        /// <summary>
        /// Will convert the specified <see cref="LoremIpsumConfiguration"/> into a compressed byte array.
        /// </summary>
        /// <param name="config">The <see cref="LoremIpsumConfiguration"/> to convert.</param>
        /// <returns>The specified <see cref="LoremIpsumConfiguration"/> converted into a compressed byte array.</returns>
        public static byte[] ConvertResource(LoremIpsumConfiguration config) => (new Compression.DeflateStreamCompressor())
                                                                                 .Compress((new Converters.TypeSerializer<LoremIpsumConfiguration>())
                                                                                 .ToByteArray(config));
        /// <summary>
        /// Will convert the specified compressed byte array into a <see cref="LoremIpsumConfiguration"/> class.
        /// </summary>
        /// <param name="source">The compressed byte array to convert.</param>
        /// <returns>The  compressed byte array converted into a <see cref="LoremIpsumConfiguration"/> class.</returns>
        public static LoremIpsumConfiguration ConvertResource(byte[] source) => (new Converters.TypeSerializer<LoremIpsumConfiguration>())
                                                                                 .FromByteArray((new Compression.DeflateStreamCompressor())
                                                                                 .Decompress(source));
        // ******** 
        /// <summary>
        /// Represents the first sentence for the default, embedded, lorem ipsum resource.
        /// </summary>
        public static string FirstSentence => GetEmbeddedResource().FirstSentence;

        // ******** CHAPTER
        /// <summary>
        /// Will generate a random chapter from the default, embedded, resource.
        /// </summary>
        /// <param name="minParagraphCount">The minimum number of paragraphs to generate for this chapter.</param>
        /// <param name="maxParagraphCount">The maximum number of paragraphs to generate for this chapter.</param>
        /// <param name="includeFirstLoremIpsumSentence"><b>True</b> to have the first sentence of the first paragraph be the first sentence in the default, embedded, resource; otherwise, <b>false</b>.</param>
        /// <returns>A random chapter generated from the default, embedded, resource.</returns>
        public static string GenerateChapter(int minParagraphCount,
                                             int maxParagraphCount,
                                             bool includeFirstLoremIpsumSentence) => GenerateChapter(RandomValue(minParagraphCount, maxParagraphCount), includeFirstLoremIpsumSentence);
        /// <summary>
        /// Will generate a random chapter from the default, embedded, resource.
        /// </summary>
        /// <param name="paragraphCount">The number of random paragraphs the chapter will have.</param>
        /// <param name="includeFirstLoremIpsumSentence"><b>True</b> to have the first sentence of the first paragraph be the first sentence in the default, embedded, resource; otherwise, <b>false</b>.</param>
        /// <returns>A random chapter generated from the default, embedded, resource.</returns>
        public static string GenerateChapter(int paragraphCount,
                                             bool includeFirstLoremIpsumSentence)
        {
            if (paragraphCount < 1)
            {
                throw new ArgumentOutOfRangeException("paragraphCount", "Parameter paragraphCount must be greater than, or equal to, one. (1 >= paragraphCount <= int.MaxValue)");
            }
            var result = new StringBuilder(_StringBuilderChapterDefaultCapacity_);
            var resource = GetEmbeddedResource();
            for (int i = 0; i < paragraphCount; i++)
            {
                var paragraphLength = Common.SequenceHelper.RandomItemFromSequenceByWeight<KeyValuePair<int, int>>(resource.ParagraphPatterns, (x) => { return x.Value; });
                result.Append(GenerateParagraph(paragraphLength.Key, includeFirstLoremIpsumSentence ? (i == 0) : false) + Environment.NewLine + Environment.NewLine);
            }
            return result.ToString().Trim();
        }
        // --------
        /// <summary>
        /// Will generate a random chapter from the given resource.
        /// </summary>
        /// <param name="resource">The resource to use to generate the random chapter.</param>
        /// <param name="minParagraphCount">The minimum number of paragraphs to generate for this chapter.</param>
        /// <param name="maxParagraphCount">The maximum number of paragraphs to generate for this chapter.</param>
        /// <param name="includeFirstLoremIpsumSentence"><b>True</b> to have the first sentence of the first paragraph be the first sentence in the <paramref name="resource"/>; otherwise, <b>false</b>.</param>
        /// <returns>A random chapter generated from the given resource.</returns>
        /// <seealso cref="CreateLoremIpsumConfiguration(string)"/>
        /// <seealso cref="CreateLoremIpsumConfiguration(string, string)"/>
        /// <seealso cref="ConvertResource(LoremIpsumConfiguration)"/>
        /// <seealso cref="ConvertResource(byte[])"/>
        public static string GenerateChapter(LoremIpsumConfiguration resource,
                                             int minParagraphCount,
                                             int maxParagraphCount,
                                             bool includeFirstLoremIpsumSentence) => GenerateChapter(resource, RandomValue(minParagraphCount, maxParagraphCount), includeFirstLoremIpsumSentence);
        /// <summary>
        /// Will generate a random chapter from the given resource.
        /// </summary>
        /// <param name="resource">The resource to use to generate the random chapter.</param>
        /// <param name="paragraphCount">The number of random paragraphs the chapter will have.</param>
        /// <param name="includeFirstLoremIpsumSentence"><b>True</b> to have the first sentence of the first paragraph be the first sentence in the <paramref name="resource"/>; otherwise, <b>false</b>.</param>
        /// <returns>A random chapter generated from the given resource.</returns>
        /// <seealso cref="CreateLoremIpsumConfiguration(string)"/>
        /// <seealso cref="CreateLoremIpsumConfiguration(string, string)"/>
        /// <seealso cref="ConvertResource(LoremIpsumConfiguration)"/>
        /// <seealso cref="ConvertResource(byte[])"/>
        public static string GenerateChapter(LoremIpsumConfiguration resource,
                                             int paragraphCount,
                                             bool includeFirstLoremIpsumSentence)
        {
            if (paragraphCount < 1)
            {
                throw new ArgumentOutOfRangeException("paragraphCount", "Parameter paragraphCount must be greater than, or equal to, one. (1 >= paragraphCount <= int.MaxValue)");
            }
            var result = new StringBuilder(_StringBuilderChapterDefaultCapacity_);
            for (int i = 0; i < paragraphCount; i++)
            {
                var paragraphLength = Common.SequenceHelper.RandomItemFromSequenceByWeight<KeyValuePair<int, int>>(resource.ParagraphPatterns, (x) => { return x.Value; });
                result.Append(GenerateParagraph(resource, paragraphLength.Key, includeFirstLoremIpsumSentence ? (i == 0) : false) + Environment.NewLine + Environment.NewLine);
            }
            return result.ToString().Trim();
        }
        // --------
        /// <summary>
        /// Will generate a random chapter from the given resource.
        /// </summary>
        /// <param name="resource">The resource to use to generate the random chapter.</param>
        /// <param name="minParagraphCount">The minimum number of paragraphs to generate for this chapter.</param>
        /// <param name="maxParagraphCount">The maximum number of paragraphs to generate for this chapter.</param>
        /// <param name="includeFirstLoremIpsumSentence"><b>True</b> to have the first sentence of the first paragraph be the first sentence in the <paramref name="resource"/>; otherwise, <b>false</b>.</param>
        /// <returns>A random chapter generated from the given resource.</returns>
        /// <seealso cref="CreateLoremIpsumConfiguration(string)"/>
        /// <seealso cref="CreateLoremIpsumConfiguration(string, string)"/>
        /// <seealso cref="ConvertResource(LoremIpsumConfiguration)"/>
        /// <seealso cref="ConvertResource(byte[])"/>
        public static string GenerateChapter(byte[] resource,
                                             int minParagraphCount,
                                             int maxParagraphCount,
                                             bool includeFirstLoremIpsumSentence) => GenerateChapter(resource, RandomValue(minParagraphCount, maxParagraphCount), includeFirstLoremIpsumSentence);
        /// <summary>
        /// Will generate a random chapter from the given resource.
        /// </summary>
        /// <param name="resource">The resource to use to generate the random chapter.</param>
        /// <param name="paragraphCount">The number of random paragraphs the chapter will have.</param>
        /// <param name="includeFirstLoremIpsumSentence"><b>True</b> to have the first sentence of the first paragraph be the first sentence in the <paramref name="resource"/>; otherwise, <b>false</b>.</param>
        /// <returns>A random chapter generated from the given resource.</returns>
        /// <seealso cref="CreateLoremIpsumConfiguration(string)"/>
        /// <seealso cref="CreateLoremIpsumConfiguration(string, string)"/>
        /// <seealso cref="ConvertResource(LoremIpsumConfiguration)"/>
        /// <seealso cref="ConvertResource(byte[])"/>
        public static string GenerateChapter(byte[] resource,
                                             int paragraphCount,
                                             bool includeFirstLoremIpsumSentence)
        {
            if (paragraphCount < 1)
            {
                throw new ArgumentOutOfRangeException("paragraphCount", "Parameter paragraphCount must be greater than, or equal to, one. (1 >= paragraphCount <= int.MaxValue)");
            }
            return GenerateChapter(ConvertResource(resource), paragraphCount, includeFirstLoremIpsumSentence);
        }

        // ******** PARAGRAPH
        /// <summary>
        /// Will generate a random paragraph from the default, embedded, resource with a random number of sentences.
        /// </summary>
        /// <param name="includeFirstLoremIpsumSentence"><b>True</b> to have the first sentence of the first paragraph be the first sentence in the default, embedded, resource; otherwise, <b>false</b>.</param>
        /// <returns>A random paragraph generated from the default, embedded, resource.</returns>
        public static string GenerateParagraph(bool includeFirstLoremIpsumSentence) => GenerateChapter(1, includeFirstLoremIpsumSentence);
        /// <summary>
        /// Will generate a random paragraph from the default, embedded, resource with a random number of sentences.
        /// </summary>
        /// <param name="minSentenceCount">The minimum number of sentences to generate for this paragraph.</param>
        /// <param name="maxSentenceCount">The maximum number of sentences to generate for this paragraph.</param>
        /// <param name="includeFirstLoremIpsumSentence"><b>True</b> to have the first sentence of the first paragraph be the first sentence in the default, embedded, resource; otherwise, <b>false</b>.</param>
        /// <returns>A random paragraph generated from the default, embedded, resource.</returns>
        public static string GenerateParagraph(int minSentenceCount,
                                               int maxSentenceCount,
                                               bool includeFirstLoremIpsumSentence) => GenerateParagraph(RandomValue(minSentenceCount, maxSentenceCount), includeFirstLoremIpsumSentence);
        /// <summary>
        /// Will generate a random paragraph from the default, embedded, resource with the number of sentence designated in the <paramref name="sentenceCount"/>.
        /// </summary>
        /// <param name="sentenceCount">The number of random sentences the paragraph will have.</param>
        /// <param name="includeFirstLoremIpsumSentence"><b>True</b> to have the first sentence of the first paragraph be the first sentence in the default, embedded, resource; otherwise, <b>false</b>.</param>
        /// <returns>A random paragraph generated from the default, embedded, resource.</returns>
        public static string GenerateParagraph(int sentenceCount,
                                               bool includeFirstLoremIpsumSentence)
        {
            if (sentenceCount < 1)
            {
                throw new ArgumentOutOfRangeException("sentenceCount", "Parameter sentenceCount must be greater than, or equal to, one. (1 >= sentenceCount <= int.MaxValue)");
            }
            var result = new StringBuilder(_StringBuilderParagraphDefaultCapacity_);
            var resource = GetEmbeddedResource();
            if (includeFirstLoremIpsumSentence)
            {
                result.Append(FirstSentence + " ");
                --sentenceCount;
            }
            for (int i = 0; i < sentenceCount; i++)
            {
                var sentenceLength = Common.SequenceHelper.RandomItemFromSequenceByWeight<KeyValuePair<int, int>>(resource.SentencePatterns, (x) => { return x.Value; });
                result.Append(GenerateSentence(sentenceLength.Key) + " ");
            }
            return result.ToString().Trim();
        }
        // --------
        /// <summary>
        /// Will generate a random paragraph from the given resource with a random number of sentences.
        /// </summary>
        /// <param name="resource">The resource to use to generate the random paragraph.</param>
        /// <param name="includeFirstLoremIpsumSentence"><b>True</b> to have the first sentence of the first paragraph be the first sentence in the <paramref name="resource"/>; otherwise, <b>false</b>.</param>
        /// <returns>A random paragraph generated from the given resource.</returns>
        /// <seealso cref="CreateLoremIpsumConfiguration(string)"/>
        /// <seealso cref="CreateLoremIpsumConfiguration(string, string)"/>
        /// <seealso cref="ConvertResource(LoremIpsumConfiguration)"/>
        /// <seealso cref="ConvertResource(byte[])"/>
        public static string GenerateParagraph(LoremIpsumConfiguration resource,
                                               bool includeFirstLoremIpsumSentence) => GenerateChapter(resource, 1, includeFirstLoremIpsumSentence);
        /// <summary>
        /// Will generate a random paragraph from the given resource with a random number of sentences.
        /// </summary>
        /// <param name="resource">The resource to use to generate the random paragraph.</param>
        /// <param name="minSentenceCount">The minimum number of sentences to generate for this paragraph.</param>
        /// <param name="maxSentenceCount">The maximum number of sentences to generate for this paragraph.</param>
        /// <param name="includeFirstLoremIpsumSentence"><b>True</b> to have the first sentence of the first paragraph be the first sentence in the <paramref name="resource"/>; otherwise, <b>false</b>.</param>
        /// <returns>A random paragraph generated from the given resource.</returns>
        /// <seealso cref="CreateLoremIpsumConfiguration(string)"/>
        /// <seealso cref="CreateLoremIpsumConfiguration(string, string)"/>
        /// <seealso cref="ConvertResource(LoremIpsumConfiguration)"/>
        /// <seealso cref="ConvertResource(byte[])"/>
        public static string GenerateParagraph(LoremIpsumConfiguration resource,
                                               int minSentenceCount,
                                               int maxSentenceCount,
                                               bool includeFirstLoremIpsumSentence) => GenerateParagraph(resource, RandomValue(minSentenceCount, maxSentenceCount), includeFirstLoremIpsumSentence);
        /// <summary>
        /// Will generate a random paragraph from the given resource with the number of sentence designated in the <paramref name="sentenceCount"/>.
        /// </summary>
        /// <param name="resource">The resource to use to generate the random paragraph.</param>
        /// <param name="sentenceCount">The number of random sentences the paragraph will have.</param>
        /// <param name="includeFirstLoremIpsumSentence"><b>True</b> to have the first sentence of the first paragraph be the first sentence in the <paramref name="resource"/>; otherwise, <b>false</b>.</param>
        /// <returns>A random paragraph generated from the given resource.</returns>
        /// <seealso cref="CreateLoremIpsumConfiguration(string)"/>
        /// <seealso cref="CreateLoremIpsumConfiguration(string, string)"/>
        /// <seealso cref="ConvertResource(LoremIpsumConfiguration)"/>
        /// <seealso cref="ConvertResource(byte[])"/>
        public static string GenerateParagraph(LoremIpsumConfiguration resource,
                                               int sentenceCount,
                                               bool includeFirstLoremIpsumSentence)
        {
            if (sentenceCount < 1)
            {
                throw new ArgumentOutOfRangeException("sentenceCount", "Parameter sentenceCount must be greater than, or equal to, one. (1 >= sentenceCount <= int.MaxValue)");
            }
            var result = new StringBuilder(_StringBuilderParagraphDefaultCapacity_);
            if (includeFirstLoremIpsumSentence)
            {
                result.Append(resource.FirstSentence + " ");
                --sentenceCount;
            }
            for (int i = 0; i < sentenceCount; i++)
            {
                var sentenceLength = Common.SequenceHelper.RandomItemFromSequenceByWeight<KeyValuePair<int, int>>(resource.SentencePatterns, (x) => { return x.Value; });
                result.Append(GenerateSentence(resource, sentenceLength.Key) + " ");
            }
            return result.ToString().Trim();
        }
        // --------
        /// <summary>
        /// Will generate a random paragraph from the given resource with a random number of sentences.
        /// </summary>
        /// <param name="resource">The resource to use to generate the random paragraph.</param>
        /// <param name="includeFirstLoremIpsumSentence"><b>True</b> to have the first sentence of the first paragraph be the first sentence in the <paramref name="resource"/>; otherwise, <b>false</b>.</param>
        /// <returns>A random paragraph generated from the given resource.</returns>
        /// <seealso cref="CreateLoremIpsumConfiguration(string)"/>
        /// <seealso cref="CreateLoremIpsumConfiguration(string, string)"/>
        /// <seealso cref="ConvertResource(LoremIpsumConfiguration)"/>
        /// <seealso cref="ConvertResource(byte[])"/>
        public static string GenerateParagraph(byte[] resource,
                                               bool includeFirstLoremIpsumSentence) => GenerateChapter(resource, 1, includeFirstLoremIpsumSentence);
        /// <summary>
        /// Will generate a random paragraph from the given resource with a random number of sentences.
        /// </summary>
        /// <param name="resource">The resource to use to generate the random paragraph.</param>
        /// <param name="minSentenceCount">The minimum number of sentences to generate for this paragraph.</param>
        /// <param name="maxSentenceCount">The maximum number of sentences to generate for this paragraph.</param>
        /// <param name="includeFirstLoremIpsumSentence"><b>True</b> to have the first sentence of the first paragraph be the first sentence in the <paramref name="resource"/>; otherwise, <b>false</b>.</param>
        /// <returns>A random paragraph generated from the given resource.</returns>
        /// <seealso cref="CreateLoremIpsumConfiguration(string)"/>
        /// <seealso cref="CreateLoremIpsumConfiguration(string, string)"/>
        /// <seealso cref="ConvertResource(LoremIpsumConfiguration)"/>
        /// <seealso cref="ConvertResource(byte[])"/>
        public static string GenerateParagraph(byte[] resource,
                                               int minSentenceCount,
                                               int maxSentenceCount,
                                               bool includeFirstLoremIpsumSentence) => GenerateParagraph(resource, RandomValue(minSentenceCount, maxSentenceCount), includeFirstLoremIpsumSentence);
        /// <summary>
        /// Will generate a random paragraph from the given resource with the number of sentence designated in the <paramref name="sentenceCount"/>.
        /// </summary>
        /// <param name="resource">The resource to use to generate the random paragraph.</param>
        /// <param name="sentenceCount">The number of random sentences the paragraph will have.</param>
        /// <param name="includeFirstLoremIpsumSentence"><b>True</b> to have the first sentence of the first paragraph be the first sentence in the <paramref name="resource"/>; otherwise, <b>false</b>.</param>
        /// <returns>A random paragraph generated from the given resource.</returns>
        /// <seealso cref="CreateLoremIpsumConfiguration(string)"/>
        /// <seealso cref="CreateLoremIpsumConfiguration(string, string)"/>
        /// <seealso cref="ConvertResource(LoremIpsumConfiguration)"/>
        /// <seealso cref="ConvertResource(byte[])"/>
        public static string GenerateParagraph(byte[] resource,
                                               int sentenceCount,
                                               bool includeFirstLoremIpsumSentence)
        {
            if (sentenceCount < 1)
            {
                throw new ArgumentOutOfRangeException("sentenceCount", "Parameter sentenceCount must be greater than, or equal to, one. (1 >= sentenceCount <= int.MaxValue)");
            }
            return GenerateParagraph(ConvertResource(resource), sentenceCount, includeFirstLoremIpsumSentence);
        }

        // ******** SENTENCE
        /// <summary>
        /// Will generate a random sentence from the default, embedded, resource with a random number of words.
        /// </summary>
        /// <returns>A randomly generated sentence from the default, embedded, resource with a random number of words.</returns>
        public static string GenerateSentence() => GenerateParagraph(1, false);
        /// <summary>
        /// Will generate a random sentence from the default, embedded, resource with a random number of words.
        /// </summary>
        /// <param name="minWordCount">The minimum number of word to generate for this sentence.</param>
        /// <param name="maxWordCount">The maximum number of word to generate for this sentence.</param>
        /// <returns>A randomly generated sentence from the default, embedded, resource with a random number of words.</returns>
        public static string GenerateSentence(int minWordCount, int maxWordCount) => GenerateSentence(RandomValue(minWordCount, maxWordCount));
        /// <summary>
        /// Will generate a random sentence from the default, embedded, resource.
        /// </summary>
        /// <param name="wordCount">The number of random words the sentence will have.</param>
        /// <returns>A random sentence generated from the default, embedded, resource.</returns>
        public static string GenerateSentence(int wordCount)
        {
            if (wordCount < 1)
            {
                throw new ArgumentOutOfRangeException("wordCount", "Parameter wordCount must be greater than, or equal to, one. (1 >= wordCount <= int.MaxValue)");
            }
            var result = new StringBuilder(_StringBuilderSentenceDefaultCapacity_);
            for (int i = 0; i < wordCount; i++)
            {
                result.Append(GenerateWord() + " ");
            }
            --result.Length;
            result[0] = char.ToUpper(result[0]);
            result.Append(GetPunctuationMark(GetEmbeddedResource()));
            return result.ToString().Trim();
        }
        // --------
        /// <summary>
        /// Will generate a random sentence from the given resource with a random number of words.
        /// </summary>
        /// <param name="resource">The resource to use to generate the random sentence.</param>
        /// <returns>A random sentence generated from the given resource.</returns>
        /// <seealso cref="CreateLoremIpsumConfiguration(string)"/>
        /// <seealso cref="CreateLoremIpsumConfiguration(string, string)"/>
        /// <seealso cref="ConvertResource(LoremIpsumConfiguration)"/>
        /// <seealso cref="ConvertResource(byte[])"/>
        public static string GenerateSentence(LoremIpsumConfiguration resource) => GenerateParagraph(resource, 1, false);
        /// <summary>
        /// Will generate a random sentence from the given resource with a random number of words.
        /// </summary>
        /// <param name="resource">The resource to use to generate the random sentence.</param>
        /// <param name="minWordCount">The minimum number of word to generate for this sentence.</param>
        /// <param name="maxWordCount">The maximum number of word to generate for this sentence.</param>
        /// <returns>A random sentence generated from the given resource.</returns>
        /// <seealso cref="CreateLoremIpsumConfiguration(string)"/>
        /// <seealso cref="CreateLoremIpsumConfiguration(string, string)"/>
        /// <seealso cref="ConvertResource(LoremIpsumConfiguration)"/>
        /// <seealso cref="ConvertResource(byte[])"/>
        public static string GenerateSentence(LoremIpsumConfiguration resource,
                                              int minWordCount,
                                              int maxWordCount) => GenerateSentence(resource, RandomValue(minWordCount, maxWordCount));
        /// <summary>
        /// Will generate a random sentence from the given resource.
        /// </summary>
        /// <param name="resource">The resource to use to generate the random sentence.</param>
        /// <param name="wordCount">The number of random words the sentence will have.</param>
        /// <returns>A random sentence generated from the given resource.</returns>
        /// <seealso cref="CreateLoremIpsumConfiguration(string)"/>
        /// <seealso cref="CreateLoremIpsumConfiguration(string, string)"/>
        /// <seealso cref="ConvertResource(LoremIpsumConfiguration)"/>
        /// <seealso cref="ConvertResource(byte[])"/>
        public static string GenerateSentence(LoremIpsumConfiguration resource,
                                              int wordCount)
        {
            if (wordCount < 1)
            {
                throw new ArgumentOutOfRangeException("wordCount", "Parameter wordCount must be greater than, or equal to, one. (1 >= wordCount <= int.MaxValue)");
            }
            var result = new StringBuilder(_StringBuilderSentenceDefaultCapacity_);
            for (int i = 0; i < wordCount; i++)
            {
                result.Append(GenerateWord(resource) + " ");
            }
            --result.Length;
            result[0] = char.ToUpper(result[0]);
            result.Append(GetPunctuationMark(resource));
            return result.ToString().Trim();
        }
        // --------
        /// <summary>
        /// Will generate a random sentence from the given resource with a random number of words.
        /// </summary>
        /// <param name="resource">The resource to use to generate the random sentence.</param>
        /// <returns>A random sentence generated from the given resource.</returns>
        /// <seealso cref="CreateLoremIpsumConfiguration(string)"/>
        /// <seealso cref="CreateLoremIpsumConfiguration(string, string)"/>
        /// <seealso cref="ConvertResource(LoremIpsumConfiguration)"/>
        /// <seealso cref="ConvertResource(byte[])"/>
        public static string GenerateSentence(byte[] resource) => GenerateParagraph(resource, 1, false);
        /// <summary>
        /// Will generate a random sentence from the given resource with a random number of words.
        /// </summary>
        /// <param name="resource">The resource to use to generate the random sentence.</param>
        /// <param name="minWordCount">The minimum number of word to generate for this sentence.</param>
        /// <param name="maxWordCount">The maximum number of word to generate for this sentence.</param>
        /// <returns>A random sentence generated from the given resource.</returns>
        /// <seealso cref="CreateLoremIpsumConfiguration(string)"/>
        /// <seealso cref="CreateLoremIpsumConfiguration(string, string)"/>
        /// <seealso cref="ConvertResource(LoremIpsumConfiguration)"/>
        /// <seealso cref="ConvertResource(byte[])"/>
        public static string GenerateSentence(byte[] resource,
                                              int minWordCount,
                                              int maxWordCount) => GenerateParagraph(resource, RandomValue(minWordCount, maxWordCount), false);
        /// <summary>
        /// Will generate a random sentence from the given resource.
        /// </summary>
        /// <param name="resource">The resource to use to generate the random sentence.</param>
        /// <param name="wordCount">The number of random words the sentence will have.</param>
        /// <returns>A random sentence generated from the given resource.</returns>
        /// <seealso cref="CreateLoremIpsumConfiguration(string)"/>
        /// <seealso cref="CreateLoremIpsumConfiguration(string, string)"/>
        /// <seealso cref="ConvertResource(LoremIpsumConfiguration)"/>
        /// <seealso cref="ConvertResource(byte[])"/>
        public static string GenerateSentence(byte[] resource,
                                              int wordCount)
        {
            if (wordCount < 1)
            {
                throw new ArgumentOutOfRangeException("wordCount", "Parameter wordCount must be greater than, or equal to, one. (1 >= wordCount <= int.MaxValue)");
            }
            return GenerateSentence(ConvertResource(resource), wordCount);
        }

        // ******** WORD
        /// <summary>
        /// Will select a random word from the words in the default, embedded, resource.
        /// </summary>
        /// <returns>A random word selected from the words in the default, embedded, resource.</returns>
        /// <seealso cref="CreateLoremIpsumConfiguration(string)"/>
        /// <seealso cref="CreateLoremIpsumConfiguration(string, string)"/>
        /// <seealso cref="ConvertResource(LoremIpsumConfiguration)"/>
        /// <seealso cref="ConvertResource(byte[])"/>
        public static string GenerateWord() => GenerateRandomWord(GetEmbeddedResource());

        /// <summary>
        /// Will select a random word from the words in the given resource.
        /// </summary>
        /// <param name="resource">The resource which contains all the word to choose from.</param>
        /// <returns>A random word selected from the words in the given resource.</returns>
        /// <seealso cref="CreateLoremIpsumConfiguration(string)"/>
        /// <seealso cref="CreateLoremIpsumConfiguration(string, string)"/>
        /// <seealso cref="ConvertResource(LoremIpsumConfiguration)"/>
        /// <seealso cref="ConvertResource(byte[])"/>
        public static string GenerateWord(LoremIpsumConfiguration resource) => GenerateRandomWord(resource);

        /// <summary>
        /// Will select a random word from the words in the given resource.
        /// </summary>
        /// <param name="resource">The resource which contains all the word to choose from.</param>
        /// <returns>A random word selected from the words in the given resource.</returns>
        /// <seealso cref="CreateLoremIpsumConfiguration(string)"/>
        /// <seealso cref="CreateLoremIpsumConfiguration(string, string)"/>
        /// <seealso cref="ConvertResource(LoremIpsumConfiguration)"/>
        /// <seealso cref="ConvertResource(byte[])"/>
        public static string GenerateWord(byte[] resource) => GenerateRandomWord(ConvertResource(resource));

        #endregion
        #region Private Methods
        private static LoremIpsumConfiguration GetEmbeddedResource()
        {
            if (_UncompressedEmbeddedResource == null)
            {
                // create (lazy load) a new WeakReference with the generated data
                _UncompressedEmbeddedResource = new WeakReference(RetrieveLoremIpsumConfiguration());
            }
            if (!(_UncompressedEmbeddedResource.Target is LoremIpsumConfiguration result))
            {
                result = RetrieveLoremIpsumConfiguration();
                _UncompressedEmbeddedResource.Target = result;
            }
            return result;
        }
        private static LoremIpsumConfiguration RetrieveLoremIpsumConfiguration() => (new Converters.TypeSerializer<LoremIpsumConfiguration>())
                                                                                     .FromByteArray((new Compression.DeflateStreamCompressor())
                                                                                     .Decompress(System.Convert
                                                                                     .FromBase64String(
                                                                                     GetEmbeddedResourceActual(
                                                                                         _EmbeddedResourceFilename,
                                                                                         System.Reflection.Assembly.GetExecutingAssembly()))));
        private static string GetEmbeddedResourceActual(string resourceName, System.Reflection.Assembly resourceAssembly)
        {
            var resourceFullName = resourceAssembly.FullName.Split(',')[0].Replace(' ', '_') + '.' + resourceName;
            using (var resourceReader = resourceAssembly.GetManifestResourceStream(resourceFullName))
            {
                if (resourceReader != null)
                {
                    using (var streamReader = new System.IO.StreamReader(resourceReader))
                    {
                        return streamReader.ReadToEnd();
                    }
                }
                return null;
            }
        }
        private static char GetPunctuationMark(LoremIpsumConfiguration resource) => Common.SequenceHelper.RandomItemFromSequenceByWeight<char>(resource.PunctuationPatterns.Keys, (x) => { return resource.PunctuationPatterns[x]; });
        private static string GenerateRandomWord(LoremIpsumConfiguration resource)
        {
            while (true)
            {
                var candidateWord = Common.SequenceHelper.RandomItemFromSequenceByWeight<string>(resource.Words.Keys, (x) => { return resource.Words[x]; });
                if (!candidateWord.Equals(_LastWordGenerated))
                {
                    _LastWordGenerated = candidateWord;
                    return candidateWord;
                }
            }
        }
        private static int RandomValue(int minCount, int maxCount)
        {
            if (minCount < 1) { throw new ArgumentOutOfRangeException("The minCount must be greater than zero. (minCount > 0).", (Exception)null); }
            if (maxCount < minCount) { throw new ArgumentOutOfRangeException("The maxCount must be greater than or equal to the minCount. (minCount <= maxCount).", (Exception)null); }
            if (_Random == null) { _Random = new Random(); }
            return _Random.Next(minCount, maxCount + 1);
        }
        #endregion
        #region Private Class: LoremIpsumTextAnalysisData
        private class LoremIpsumTextAnalysisData
        {
            public string Word
            {
                get; set;
            }
            public int Count
            {
                get; set;
            }

            public override string ToString() => $"{Count:N0}; {Word}";
        }
        #endregion
    }
}
