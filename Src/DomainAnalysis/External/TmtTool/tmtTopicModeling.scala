// Stanford TMT Example 5 - Selecting LDA model parameters
// http://nlp.stanford.edu/software/tmt/0.3/

// tells Scala where to find the TMT classes
import scalanlp.io._;
import scalanlp.stage._;
import scalanlp.stage.text._;
import scalanlp.text.tokenize._;
import scalanlp.pipes.Pipes.global._;

import edu.stanford.nlp.tmt.stage._;
import edu.stanford.nlp.tmt.model.lda._;
import edu.stanford.nlp.tmt.model.llda._;

val source = CSVFile("C:\\Users\\xlian\\Documents\\TFSDomainAnalysis\\CareTeam\\DomainAnalysis\\DomainAnalysis\\External\\TmtTool\\tmtInputFile.csv") ~> IDColumn(1);

val tokenizer = {
  SimpleEnglishTokenizer() ~>            // tokenize on space and punctuation
  CaseFolder() ~>                        // lowercase everything
  WordsAndNumbersOnlyFilter() ~>    
 // PorterStemmer()~>     // ignore non-words and non-numbers
  MinimumLengthFilter(3)                 // take terms with >=3 characters
}

val text = {
  source ~>                              // read from the source file
  Column(2) ~>                           // select column containing text
  TokenizeWith(tokenizer) ~>             // tokenize with tokenizer above 
  TermCounter() ~>                       // collect counts (needed below)
  TermDynamicStopListFilter(40) ~>       // filter out 30 most common terms
  TermStopListFilter(List("transportation","rail","railroad","pittsford","clarendon","dcn","tbd","dec","meteorcomm","street","south","management","engineer","segment","septa","review","shall","spr","aem","len","date","end","amt","haven","harlem","jun","may","apr","mar","oct","trai","tags","result","contains","know","utah","electric","western","east","eastern","southeastern","nashville","rutland","tag","etms","itcs","acses","bit","bits","bit","boolean","ldars","santa","different","city","aar","waterbury","trains","train","hudson","brc","line","metro","value","ver","west","lack","amtrak","plans","cai","time","tls","tbc","srs","atc","irs","idd","den","fig","xtermw","rspp","diego","dsr","sync","defined","pennsylvania","table","itc","case","tests","psi","mtea","datums","set","etms","ptcip","cfr","described","technical","based","rcl","kcsr","fra","differences","required","proper","reserved","support","configuration","following","technology","technologies","federal","industry","experts","potential","defines","notes","implemented","page","provides","jct","cad","ceq","wgs","ttci","scac","cbtc","special","csx","redacted","steps","hadson","used","number","requirements","development","documentation","items","program","datum","earth","recommended","practices","challenge","standard","practices","architecture","year","southern","company","millions","north","clean","note","specification","col","operations","products","total","san","sounder","caltrain","interoperability","existing","rights","system","systems","figure","chicago","metra","vermont","island","controlled","kansas","deployment","southern","service","cab","wabtec","valid","external","indicate","element","architecture","sensitive","release","government","administration","bch","pacific","norfolk","factor","table","plan","material","canadian","group","critical","dgno","exception","installation","script","version","paragraph","work","certain","effective","october","hot","copyright","important","using","clear","crossing","thi","practices","","bnsf","about","above","across","after","afterwards","again","against","all","almost","alone","along","already","also","although","always","among","amongst","amoungst","amount","and","another","any","anyhow","anyone","anything","anyway","anywhere","are","around","back","became","because","become","becomes","becoming","been","before","beforehand","behind","being","below","beside","besides","between","beyond","bill","both","bottom","but","call","can","cannot","cant","computer","con","could","couldnt","cry","describe","detail","done","down","due","during","each","eight","either","eleven","else","elsewhere","empty","enough","etc","even","ever","every","everyone","everything","everywhere","except","few","fifteen","fify","fill","find","fire","first","five","for","former","formerly","forty","found","four","from","front","full","further","get","give","had","has","hasnt","have","hence","her","here","hereafter","hereby","herein","hereupon","hers","herself","him","himself","his","how","however","hundred","inc","indeed","interest","into","its","itself","keep","last","latter","latterly","least","less","ltd","made","many","may","meanwhile","might","mill","mine","more","moreover","most","mostly","move","much","must","myself","name","namely","neither","never","nevertheless","next","nine","nobody","none","noone","nor","not","nothing","now","nowhere","off","often","once","one","only","onto","other","others","otherwise","our","ours","ourselves","out","over","own","part","per","perhaps","please","put","rather","same","see","seem","seemed","seeming","seems","serious","several","she","should","show","side","since","sincere","six","sixty","some","somehow","someone","something","sometime","sometimes","somewhere","still","such","system","take","ten","than","that","the","their","them","themselves","then","thence","there","thereafter","thereby","therefore","therein","thereupon","these","they","thick","thin","third","this","those","though","three","through","throughout","thru","thus","together","too","top","toward","towards","twelve","twenty","two","under","until","upon","very","via","was","well","were","what","whatever","when","whence","whenever","where","whereafter","whereas","whereby","wherein","whereupon","wherever","whether","which","while","whither","who","whoever","whole","whom","whose","why","will","with","within","without","would","yet","you","your","yours","yourself","yourselves","java","class","false","true","finally","void","string","static","integer","interface","final","char","short","int","catch","public","else","double","break","this","private","float","goto","default","apache","code","param","abstract","continue","for","http","wiki","href","doc","html","name","general","class","unsigned","yes","july","operating","testing","code","unit","geodetic","reference","international","application","ment","file","design","requirement","implementation","make","car","cars","segments","use","rev","software","document","project","non","bit","type","sixth","september","mph","author","include","new","sub","instal","syr","swr","sch","per","need","appendix","ton","ten","per","none","type","section","safety","implement","approach","protect","sec","april","practic","level","june","jan","feb","main","function","stop","llc","null","varchar","cim","indic","ter")) ~>
  TermMinimumDocumentCountFilter(4) ~>   // filter terms in <4 docs
  DocumentMinimumLengthFilter(5)         // take only docs with >=5 terms
}

// set aside 80 percent of the input text as training data ...
val numTrain = text.data.size * 4 / 5;

// build a training dataset
val training = LDADataset(text ~> Take(numTrain));
 
// build a test dataset, using term index from the training dataset 
val testing  = LDADataset(text ~> Drop(numTrain));

// a list of pairs of (number of topics, perplexity)
case class Score(numTopics: Int, perplexity: Double)
var scores = List.empty[Score];

// loop over various numbers of topics, training and evaluating each model
// PlaceHolderTopicNumberArrary is like : 5,10,15,20,25,30,35,40
//for (numTopics <- List({PlaceHolderTopicNumberArray})) {
  for (numTopics <- List(25,30,35,40)) {
  val params = LDAModelParams(numTopics = numTopics, dataset = training);
  //val output = file("lda-"+training.signature+"-"+params.signature);
  val model = TrainCVB0LDA(params, training, output=null, maxIterations=500);
  
  println("[perplexity] computing at "+numTopics);

  val perplexity = model.computePerplexity(testing);
  
  println("[perplexity] perplexity at "+numTopics+" topics: "+perplexity);
  val score = Score(numTopics, perplexity);
  scores :+= score;
}

//for (score <- scores) {
//  scala.tools.nsc.io.File("C:\\Users\\xlian\\Documents\\TmtTool\\tmtOutputInfo.txt", true).writeAll("[perplexity] perplexity at "+score.numTopics+" topics: "+score.perplexity+"\n");  
//}

// find Minimum perplexity with numTopics
def min(s1: Score, s2: Score): Score = if (s1.perplexity < s2.perplexity) s1 else s2
val minScore = scores.reduceRight(min)

//turn the text into a dataset ready to be used with LDA
val dataset = LDADataset(text);

//define the model parameters, numTopics, Smoothing=SymmetricDirichletParams(.1), topicSmoothing=SymmetricDirichletParams(.1)
val params = LDAModelParams(numTopics = minScore.numTopics, dataset = dataset); //the original numTopics=30

val modelPath = file("tmtOutput");

//train the model
//TrainCVB0LDA(params,dataset,output=modelPath, maxIterations={MaxIterationPlaceHolder});
  TrainCVB0LDA(params,dataset,output=modelPath, maxIterations=1500);
//if use the Gibbs sampler for inference, instead use
//TrainGibbsLDA(params, dataset,output=modelpath, maxIterations={MaxIterationPlaceHolder});

scala.tools.nsc.io.File("C:\\Users\\xlian\\Documents\\TFSDomainAnalysis\\CareTeam\\DomainAnalysis\\DomainAnalysis\\External\\TmtTool\\tmtOutputInfo.txt").writeAll("MinimumTopic: "+minScore.numTopics);