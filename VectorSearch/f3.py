from dotenv     import load_dotenv

from langchain_community.document_loaders   import PyPDFLoader
from langchain_core.output_parsers          import StrOutputParser
from langchain_core.prompts                 import ChatPromptTemplate
from langchain_core.runnables               import RunnablePassthrough
from langchain_openai                       import ChatOpenAI, OpenAIEmbeddings
from langchain_text_splitters               import RecursiveCharacterTextSplitter
from langchain_community.vectorstores       import Cassandra

from cassandra.cluster import Cluster
import cassio

load_dotenv()

embedding = OpenAIEmbeddings()

cluster = Cluster(["127.0.0.1"], connect_timeout = 10)
session = cluster.connect()


CASSANDRA_KEYSPACE = "vectortest1" 
print(f"using keyspace {CASSANDRA_KEYSPACE}")
cassio.init(session = session, keyspace = CASSANDRA_KEYSPACE)
vstore = Cassandra(
    embedding= embedding,
    table_name= "cassandra_vector_demo"
)

pdf_loader = PyPDFLoader("f3.pdf")
splitter = RecursiveCharacterTextSplitter(chunk_size=512, chunk_overlap=64)
docs_from_pdf = pdf_loader.load_and_split(text_splitter=splitter)

print(f"Documents from PDF: {len(docs_from_pdf)}.")
inserted_ids_from_pdf = vstore.add_documents(docs_from_pdf)
print(f"Inserted {len(inserted_ids_from_pdf)} documents.")

retriever = vstore.as_retriever(search_kwargs={"k": 3})

photographer_template = """
You are a photographer with great technical knowledge about photocameras that uses this knowledge
to craft well-thought answers to user questions. Use the provided context as the basis
for your answers and do not make up new reasoning paths - just mix-and-match what you are given.
Your answers must be concise and to the point, and refrain from answering about other topics than photography.

CONTEXT:
{context}

QUESTION: {question}

YOUR ANSWER:"""

photo_prompt = ChatPromptTemplate.from_template(photographer_template)

llm = ChatOpenAI()
print(llm)
chain = (
    {"context": retriever, "question": RunnablePassthrough()}
    | photo_prompt
    | llm
    | StrOutputParser()
)

answer = chain.invoke("How should I load a roll of film into a Nikon?")
print(answer)
answer = chain.invoke("How do I do a multiple expose shot?")
print(answer)

# exit(0)





